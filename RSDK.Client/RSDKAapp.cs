using Revuo.Chat.Abstraction.Client;
using Revuo.Chat.Abstraction.Extensions;
using Revuo.Chat.Base;
using Revuo.Chat.Client.Base.Abstractions;
using Revuo.Chat.Common;
using Revuo.Chat.Base.I18N;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace RSDK.Client;

public class SDKApp : BaseThinClientApp
{
    public SDKApp() : base(new StaticTranslator(I18N.set))
    {
    }
 
    protected override Task OnInit()
    {
        // register existing action + control
        this.AddAction<NewProjectResponse>(NewProject);
        this.AddAction<NewProjectResponse, ProjectCreateProgress>(CreateNewProject);
        this.AddAction<ProjectCreateProgress, ProjectCreateProgress>(CreateNewProject_CreateFolder);
        this.AddAction<ProjectCreateProgress, ProjectCreateProgress>(CreateNewProject_DotnetNew);
        this.AddAction<ProjectCreateProgress, ProjectCreateProgress>(CreateNewProject_CopyHowtos);
        this.AddAction<ProjectCreateProgress, ProjectCreateProgress>(CreateNewProject_CreateSpecificFiles);
        this.AddControl<NewProjectControl>();

        // SDK settings (default folder for new projects)
        this.AddAction<SdkSettings>(GetSdkSettings);
        this.AddAction<SdkSettings>(SaveSdkSettings);
        this.AddControl<SdkSettingsControl>();
        this.AddControl<ProjectCreateProgressControl>();

        return Task.CompletedTask;
    }

    private async Task<ProjectCreateProgress> CreateNewProject(IThinClientContext context, NewProjectResponse newProjectRequest)
    {
        //1. create folder
        //2. run dotnet new to create classlib for razor framework 10
        //3. copy howto's
        //4. create specific files: (take them from current project, embeed them as resources or have them 
        //   as strings in code and write them out):
        // a. github pipilines
        // b. vscode launch and tasks
        // c. revuo files to run workflows for installations and installation
        //5. create simple application
        //6. use StaticTranslator for transaltions in this simple app
        //7. add one simple control
        
        
        // all those actions above can be separte actions that are run in sequence, 
        // and report progress to the user via user control of creations. 
        // This allows for better error handling and user feedback.

        var steps = new []{
            nameof(CreateNewProject_CreateFolder),
            nameof(CreateNewProject_DotnetNew),
            nameof(CreateNewProject_CopyHowtos),
            nameof(CreateNewProject_CreateSpecificFiles),
        };

        var stepResult = new ProjectCreateProgress();
        stepResult.NewProjectRequest = newProjectRequest;
        stepResult.Culture = context.CurrentCulture;

        for (var i = 0; i < steps.Length; i++)
        {
            var step = steps[i];

            // baseline progress for this step (don't override higher values set by the step itself)
            var startPercent = (int)Math.Floor(i * 100.0 / steps.Length);
            var endPercent = (int)Math.Ceiling((i + 1) * 100.0 / steps.Length);
            stepResult.Percent = Math.Max(stepResult.Percent, startPercent);

            stepResult.SetStep(
                this.Translator,
                context.CurrentCulture,
                step);

            stepResult = await context.RunAction(
                "RSDK.Client.SDKApp",
                step, stepResult) as ProjectCreateProgress;

            // ensure progress advances at least to the endPercent for this step
            stepResult.Percent = Math.Max(stepResult.Percent, endPercent);

            if (stepResult!.IsError())
                return stepResult!;
        }
        
        return stepResult!;
    }

    private Task<ProjectCreateProgress> CreateNewProject_CreateFolder(IThinClientContext context, ProjectCreateProgress result)
    {
        // if folder exists than fail with error message "Folder already exists"
        if (Directory.Exists(result.NewProjectRequest.ProjectPath))
        {
            result.WithError(
                this.Translator,
                result.Culture, 
                "ERROR_FOLDER_EXISTS_0", result.NewProjectRequest.ProjectPath);
            return Task.FromResult(result);
        }

        // let's create the folder 
        Directory.CreateDirectory(result.NewProjectRequest.ProjectPath);

        return Task.FromResult(result);
    }

    private async Task<ProjectCreateProgress> CreateNewProject_DotnetNew(IThinClientContext context, ProjectCreateProgress result)
    {
        // run `dotnet new razorclasslib -f net10.0` inside the project folder (create files in existing folder)
        var projectPath = result.NewProjectRequest.ProjectPath;
        var projectName = string.IsNullOrWhiteSpace(result.NewProjectRequest.ProjectName)
            ? System.IO.Path.GetFileName(projectPath.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar))
            : result.NewProjectRequest.ProjectName;

        result.SetStep(this.Translator, result.Culture, nameof(CreateNewProject_DotnetNew));
        result.Percent = 25;

        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo("dotnet", $"new razorclasslib -n \"{projectName}\" -f net10.0 -o .")
            {
                WorkingDirectory = projectPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var p = System.Diagnostics.Process.Start(psi)!;
            var stdout = await p.StandardOutput.ReadToEndAsync();
            var stderr = await p.StandardError.ReadToEndAsync();
            await p.WaitForExitAsync();

            if (!string.IsNullOrWhiteSpace(stdout))
                result.Log.Add(stdout);
            if (!string.IsNullOrWhiteSpace(stderr))
                result.Log.Add(stderr);

            if (p.ExitCode != 0)
            {
                result.WithError(this.Translator, result.Culture, "ERROR_DOTNET_NEW_FAILED_0", stderr);
                return result;
            }

            result.Percent = 45;
            return result;
        }
        catch (Exception ex)
        {
            result.WithError(this.Translator, result.Culture, "ERROR_EXCEPTION_0", ex.Message);
            return result;
        }
    }

    private async Task<ProjectCreateProgress> CreateNewProject_CopyHowtos(IThinClientContext context, ProjectCreateProgress result)
    {
        // extract Howto files from embedded resources (preferred) or fall back to disk copy
        result.SetStep(this.Translator, result.Culture, nameof(CreateNewProject_CopyHowtos));
        result.Percent = 50;

        try
        {
            var asm = typeof(SDKApp).Assembly;
            var resourceNames = asm.GetManifestResourceNames()
                                   .Where(n => n.IndexOf(".Howto.", StringComparison.OrdinalIgnoreCase) >= 0)
                                   .ToArray();

            if (resourceNames.Length > 0)
            {
                var targetHowto = Path.Combine(result.NewProjectRequest.ProjectPath, "Howto");
                Directory.CreateDirectory(targetHowto);

                foreach (var resourceName in resourceNames)
                {
                    var rest = resourceName.Substring(resourceName.IndexOf(".Howto.", StringComparison.OrdinalIgnoreCase) + ".Howto.".Length);
                    var parts = rest.Split('.');

                    string fileName;
                    string[] dirParts;

                    if (parts.Length >= 2)
                    {
                        fileName = parts[^2] + "." + parts[^1];
                        dirParts = parts.Length > 2 ? parts.Take(parts.Length - 2).ToArray() : Array.Empty<string>();
                    }
                    else
                    {
                        fileName = rest;
                        dirParts = Array.Empty<string>();
                    }

                    var relPath = dirParts.Length > 0 ? Path.Combine(Path.Combine(dirParts), fileName) : fileName;
                    var dest = Path.Combine(targetHowto, relPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);

                    using var rs = asm.GetManifestResourceStream(resourceName);
                    using var fs = File.Create(dest);
                    await rs!.CopyToAsync(fs);

                    result.Log.Add($"Extracted resource: {relPath}");
                }

                result.Percent = 60;
                return result;
            }

            // fallback: copy from repo/working directory (existing behaviour)
            string? howtoSource = null;
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "Howto");
                if (Directory.Exists(candidate))
                {
                    howtoSource = candidate;
                    break;
                }

                dir = dir.Parent;
            }

            if (howtoSource == null)
            {
                var cwdCandidate = Path.Combine(Directory.GetCurrentDirectory(), "Howto");
                if (Directory.Exists(cwdCandidate))
                    howtoSource = cwdCandidate;
            }

            if (howtoSource == null)
            {
                result.Log.Add("No Howto folder found; skipping copy.");
                result.Percent = 55;
                return result;
            }

            var targetHowto2 = Path.Combine(result.NewProjectRequest.ProjectPath, "Howto");
            Directory.CreateDirectory(targetHowto2);

            foreach (var file in Directory.EnumerateFiles(howtoSource, "*", SearchOption.AllDirectories))
            {
                var rel = Path.GetRelativePath(howtoSource, file);
                var dest = Path.Combine(targetHowto2, rel);
                Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                File.Copy(file, dest, true);
                result.Log.Add($"Copied: {rel}");
            }

            result.Percent = 60;
            return result;
        }
        catch (Exception ex)
        {
            result.WithError(this.Translator, result.Culture, "ERROR_COPY_HOWTO_0", ex.Message);
            return result;
        }
    }

    private async Task<ProjectCreateProgress> CreateNewProject_CreateSpecificFiles(IThinClientContext context, ProjectCreateProgress result)
    {
        // step 4 — create specific helper files:
        // a) GitHub workflows
        // b) VS Code launch/tasks
        // c) Revuo workflow / InstallationRequest files

        result.SetStep(this.Translator, result.Culture, nameof(CreateNewProject_CreateSpecificFiles));
        result.Percent = Math.Max(result.Percent, 60);

        try
        {
            var projectPath = result.NewProjectRequest.ProjectPath;
            var projectName = string.IsNullOrWhiteSpace(result.NewProjectRequest.ProjectName)
                ? Path.GetFileName(projectPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                : result.NewProjectRequest.ProjectName;

            // --- a) GitHub workflow ---
            var ghDir = Path.Combine(projectPath, ".github", "workflows");
            Directory.CreateDirectory(ghDir);
            var ghWorkflowPath = Path.Combine(ghDir, "ci.yml");
            var ghWorkflow = $@"name: .NET CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build --configuration Release --no-restore
";
            File.WriteAllText(ghWorkflowPath, ghWorkflow);
            result.Log.Add($"Created GitHub workflow: {Path.GetRelativePath(projectPath, ghWorkflowPath)}");
            result.Percent = 70;

            // --- b) VSCode launch + tasks ---
            var vsDir = Path.Combine(projectPath, ".vscode");
            Directory.CreateDirectory(vsDir);

            var tasksJson = $@"{{
  ""version"": ""2.0.0"",
  ""tasks"": [
    {{
      ""type"": ""process"",
      ""label"": ""build {projectName}"",
      ""command"": ""dotnet"",
      ""args"": [ ""build"", ""${{workspaceFolder}}/{projectName}/{projectName}.csproj"", ""-c"", ""Debug"" ],
      ""group"": ""build"",
      ""presentation"": {{ ""echo"": true, ""reveal"": ""always"", ""focus"": false, ""panel"": ""shared"", ""showReuseMessage"": true, ""clear"": false }},
      ""problemMatcher"": ""$msCompile""
    }},
    {{
      ""type"": ""process"",
      ""label"": ""build"",
      ""command"": ""dotnet"",
      ""args"": [ ""build"", ""${{workspaceFolder}}/{projectName}/{projectName}.csproj"", ""-c"", ""Debug"" ],
      ""group"": ""build"",
      ""presentation"": {{ ""echo"": true, ""reveal"": ""always"", ""focus"": false, ""panel"": ""shared"", ""showReuseMessage"": true, ""clear"": false }},
      ""problemMatcher"": ""$msCompile""
    }},
    {{
      ""type"": ""process"",
      ""label"": ""test {projectName}"",
      ""command"": ""dotnet"",
      ""args"": [ ""test"", ""${{workspaceFolder}}/{projectName}/{projectName}.csproj"", ""-c"", ""Debug"" ],
      ""group"": ""test"",
      ""presentation"": {{ ""echo"": true, ""reveal"": ""always"", ""focus"": false, ""panel"": ""shared"", ""showReuseMessage"": true, ""clear"": false }}
    }},
    {{
      ""type"": ""process"",
      ""label"": ""test"",
      ""command"": ""dotnet"",
      ""args"": [ ""test"" ],
      ""group"": ""test"",
      ""presentation"": {{ ""echo"": true, ""reveal"": ""always"", ""focus"": false, ""panel"": ""shared"", ""showReuseMessage"": true, ""clear"": false }}
    }}
  ]
}}";

            File.WriteAllText(Path.Combine(vsDir, "tasks.json"), tasksJson);

            var launchJson = $@"{{
  ""version"": ""0.2.0"",
  ""configurations"": [
    {{
      ""name"": ""C#: {projectName} Debug"",
      ""type"": ""coreclr"",
      ""request"": ""attach"",
      ""processName"": ""{projectName}.exe"",
      ""preLaunchTask"": ""Run Workflow""
    }},
    {{
      ""name"": ""Launch {projectName}"",
      ""type"": ""coreclr"",
      ""request"": ""launch"",
      ""preLaunchTask"": ""Run Workflow"",
      ""program"": ""${{workspaceFolder}}/bin/Debug/net10.0/{projectName}.dll"",
      ""args"": [],
      ""cwd"": ""${{workspaceFolder}}"",
      ""stopAtEntry"": false
    }}
  ]
}}";

            File.WriteAllText(Path.Combine(vsDir, "launch.json"), launchJson);
            result.Log.Add("Created VSCode settings: .vscode/launch.json + .vscode/tasks.json");
            result.Percent = 80;

            // --- c) Revuo installation / workflow files ---
            var instRequest = new
            {
                Token = (string?)null,
                TypeName = "Installer.Model.InstallationRequest",
                D = new
                {
                    DllPath = Path.Combine(projectPath, "bin", "Debug", "net10.0", projectName + ".dll"),
                    Culture = result.Culture ?? string.Empty
                }
            };

            var instJson = System.Text.Json.JsonSerializer.Serialize(instRequest, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            var instPath = Path.Combine(projectPath, "InstallationRequest.json");
            File.WriteAllText(instPath, instJson);

            var workflow = new
            {
                Name = "Install application from dll",
                Description = "Install application, check if it is visible on list and run some action.",
                Steps = new object[]
                {
                    new { Order = 1, ApplicationName = "Installer.Client.InstallerApp", ActionName = "InstallFromDll", ParameterFilePath = instPath, ContinueOnError = true },
                    new { Order = 2, ApplicationName = "Installer.Client.InstallerApp", ActionName = "ListApplications", ParameterFilePath = (string?)null, ContinueOnError = true },
                    new { Order = 3, ApplicationName = "RSDK.Client.SDKApp", ActionName = "SelectNewProjectType", ParameterFilePath = (string?)null, ContinueOnError = true }
                }
            };

            var workflowJson = System.Text.Json.JsonSerializer.Serialize(workflow, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            var workflowPath = Path.Combine(projectPath, "InstallRSDKWorkflow.json");
            File.WriteAllText(workflowPath, workflowJson);
            result.Log.Add("Created Revuo workflow files: InstallationRequest.json + InstallRSDKWorkflow.json");

            result.Percent = 95;
            return result;
        }
        catch (Exception ex)
        {
            result.WithError(this.Translator, result.Culture, "ERROR_CREATE_FILES_0", ex.Message);
            return result;
        }
    }

    private async Task<NewProjectResponse> NewProject(IThinClientContext context)
    {
        var response = new NewProjectResponse
        {
            ProjectType = ProjectType.CSharp,
        };

        try
        {
            // attempt to read stored default folder from device storage (key: sdk.defaultNewProjectFolder)
            dynamic dctx = context;
            var stored = await dctx.devicestorage.GetAsync<string>("sdk.defaultNewProjectFolder");
            if (!string.IsNullOrWhiteSpace(stored))
                response.ProjectPath = stored!;
        }
        catch
        {
            // ignore — fall back to user's Documents folder if nothing stored
        }

        if (string.IsNullOrWhiteSpace(response.ProjectPath))
            response.ProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        return response;
    }

    // Returns current SDK settings (reads device storage)
    public async Task<SdkSettings> GetSdkSettings(IThinClientContext context)
    {
        var s = await context.DeviceStorage!.Get<SdkSettings>(SdkSettings.Key) ?? new SdkSettings();
        if(string.IsNullOrWhiteSpace(s.DefaultNewProjectFolder))
            s.DefaultNewProjectFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    
        return s;
    }

    // Persist SDK settings (stores into device storage)
    private async Task<SdkSettings> SaveSdkSettings(IThinClientContext context, SdkSettings request)
    {
        request.Id = SdkSettings.Key; // ensure the key is set for storage
        await context.DeviceStorage!.Store(request);
        return request;
    }
}
