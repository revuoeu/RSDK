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
using System;
using System.Threading.Tasks;

namespace RSDK.Client;

public partial class SDKApp
{
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
            nameof(CreateNewProject_CreateReadme),
            nameof(CreateNewProject_CreateSolution),
            nameof(CreateNewProject_CreateGitIgnore),
            nameof(CreateNewProject_CreateRevuoApp),
            
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

        var rundotnetnew =  await RunCommand(result, projectPath, projectName, "dotnet", $"new razorclasslib -f net10.0 -n \"{projectName}\" -o .");
        if(rundotnetnew.IsError())
            return rundotnetnew;

        var rundotnetpackageinstall =  await RunCommand(result, projectPath, projectName, "dotnet", $"package update");

        return rundotnetpackageinstall;
    }

    private async Task<ProjectCreateProgress> RunCommand(ProjectCreateProgress result,
        string projectPath, 
        string projectName,
        string command,
        string args)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo(command, args)
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
            // helper: load text templates embedded under the `Templates` folder in the assembly
            static string? LoadTemplateFromAssembly(string name)
            {
                var asm = typeof(SDKApp).Assembly;
                var rn = asm.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith($".Templates.{name}", StringComparison.OrdinalIgnoreCase));
                if (rn == null) return null;
                using var rs = asm.GetManifestResourceStream(rn)!;
                using var sr = new StreamReader(rs);
                return sr.ReadToEnd();
            }
            var projectPath = result.NewProjectRequest.ProjectPath;
            var projectName = string.IsNullOrWhiteSpace(result.NewProjectRequest.ProjectName)
                ? Path.GetFileName(projectPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                : result.NewProjectRequest.ProjectName;

            // --- a) GitHub workflow ---
            var ghDir = Path.Combine(projectPath, ".github", "workflows");
            Directory.CreateDirectory(ghDir);
            var ghWorkflowPath = Path.Combine(ghDir, "ci.yml");
            var ghTemplate = LoadTemplateFromAssembly("ci.yml.tpl");
            var ghWorkflow = !string.IsNullOrEmpty(ghTemplate)
                ? ghTemplate
                : $@"name: .NET CI

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

            var tasksTemplate = LoadTemplateFromAssembly("tasks.json.tpl");
            var tasksText = tasksTemplate.Replace("{{ProjectName}}", projectName);
                

            File.WriteAllText(Path.Combine(vsDir, "tasks.json"), tasksText);

            var launchTemplate = LoadTemplateFromAssembly("launch.json.tpl");
            var launchText =
                 launchTemplate.Replace("{{ProjectName}}", projectName);
                

            File.WriteAllText(Path.Combine(vsDir, "launch.json"), launchText);
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

            var workflowTemplate = LoadTemplateFromAssembly("InstallRSDKWorkflow.json.tpl");
            string workflowJson;
            if (!string.IsNullOrEmpty(workflowTemplate))
            {
                // insert JSON-escaped absolute path for installation request
                workflowJson = workflowTemplate.Replace("{{InstallationRequestPathJson}}", System.Text.Json.JsonSerializer.Serialize(instPath));
            }
            else
            {
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

                workflowJson = System.Text.Json.JsonSerializer.Serialize(workflow, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }

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

    private async Task<ProjectCreateProgress> CreateNewProject_CreateReadme(IThinClientContext context, ProjectCreateProgress result)
    {
        result.SetStep(this.Translator, result.Culture, nameof(CreateNewProject_CreateReadme));
        result.Percent = Math.Max(result.Percent, 95);

        try
        {
            var projectPath = result.NewProjectRequest.ProjectPath;
            var projectName = string.IsNullOrWhiteSpace(result.NewProjectRequest.ProjectName)
                ? Path.GetFileName(projectPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                : result.NewProjectRequest.ProjectName;

            var readmePath = Path.Combine(projectPath, "README.md");
            var readme = $"# {projectName}\n\nGenerated by RSDK.\n\nTarget: .NET 10.0\n\nBuild: `dotnet build`\n\nSee the `Howto` folder for examples and guidance.";
            File.WriteAllText(readmePath, readme);
            result.Log.Add($"Created README: {Path.GetRelativePath(projectPath, readmePath)}");

            result.Percent = 96;
            return result;
        }
        catch (Exception ex)
        {
            result.WithError(this.Translator, result.Culture, "ERROR_CREATE_README_0", ex.Message);
            return result;
        }
    }

    private async Task<ProjectCreateProgress> CreateNewProject_CreateSolution(IThinClientContext context, ProjectCreateProgress result)
    {
        result.SetStep(this.Translator, result.Culture, nameof(CreateNewProject_CreateSolution));
        result.Percent = Math.Max(result.Percent, 96);

        try
        {
            var projectPath = result.NewProjectRequest.ProjectPath;
            var projectName = string.IsNullOrWhiteSpace(result.NewProjectRequest.ProjectName)
                ? Path.GetFileName(projectPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                : result.NewProjectRequest.ProjectName;

            // create a local solution and add the project
            var psi = new ProcessStartInfo("dotnet", $"new sln -n \"{projectName}\"")
            {
                WorkingDirectory = projectPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var p = Process.Start(psi)!)
            {
                var stdout = await p.StandardOutput.ReadToEndAsync();
                var stderr = await p.StandardError.ReadToEndAsync();
                await p.WaitForExitAsync();

                if (p.ExitCode != 0)
                {
                    result.WithError(this.Translator, result.Culture, "ERROR_CREATE_SOLUTION_0", stderr);
                    return result;
                }

                if (!string.IsNullOrWhiteSpace(stdout)) result.Log.Add(stdout);
                if (!string.IsNullOrWhiteSpace(stderr)) result.Log.Add(stderr);
            }

            var psi2 = new ProcessStartInfo("dotnet", $"sln add \"{projectName}.csproj\"")
            {
                WorkingDirectory = projectPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var p2 = Process.Start(psi2)!)
            {
                var stdout2 = await p2.StandardOutput.ReadToEndAsync();
                var stderr2 = await p2.StandardError.ReadToEndAsync();
                await p2.WaitForExitAsync();

                if (p2.ExitCode != 0)
                {
                    result.WithError(this.Translator, result.Culture, "ERROR_ADD_PROJECT_TO_SOLUTION_0", stderr2);
                    return result;
                }

                if (!string.IsNullOrWhiteSpace(stdout2)) result.Log.Add(stdout2);
                if (!string.IsNullOrWhiteSpace(stderr2)) result.Log.Add(stderr2);
            }

            result.Percent = 98;
            return result;
        }
        catch (Exception ex)
        {
            result.WithError(this.Translator, result.Culture, "ERROR_CREATE_SOLUTION_0", ex.Message);
            return result;
        }
    }

    private async Task<ProjectCreateProgress> CreateNewProject_CreateGitIgnore(IThinClientContext context, ProjectCreateProgress result)
    {
        result.SetStep(this.Translator, result.Culture, nameof(CreateNewProject_CreateGitIgnore));
        result.Percent = Math.Max(result.Percent, 98);

        try
        {
            var projectPath = result.NewProjectRequest.ProjectPath;
            var gitignore = @"# Build directories
bin/
obj/

# Visual Studio
.vs/
*.user
*.suo

# Rider
.idea/

# VS Code
.vscode/

# NuGet
*.nupkg
packages/

# OS
.DS_Store
Thumbs.db
";
            File.WriteAllText(Path.Combine(projectPath, ".gitignore"), gitignore);
            result.Log.Add("Created .gitignore");

            result.Percent = 100;
            return result;
        }
        catch (Exception ex)
        {
            result.WithError(this.Translator, result.Culture, "ERROR_CREATE_GITIGNORE_0", ex.Message);
            return result;
        }
    }

    private async Task<ProjectCreateProgress> CreateNewProject_CreateRevuoApp(IThinClientContext context, ProjectCreateProgress result)
    {
        result.SetStep(this.Translator, result.Culture, nameof(CreateNewProject_CreateRevuoApp));
        result.Percent = Math.Max(result.Percent, 99);

        try
        {
            var projectPath = result.NewProjectRequest.ProjectPath;
            var projectName = string.IsNullOrWhiteSpace(result.NewProjectRequest.ProjectName)
                ? Path.GetFileName(projectPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                : result.NewProjectRequest.ProjectName;

            // 1) update project file to add Revuo package references (if not already present)
            var projFile = Path.Combine(projectPath, projectName + ".csproj");
            if (File.Exists(projFile))
            {
                var projText = File.ReadAllText(projFile);
                if (!projText.Contains("Revuo.Chat.Client.Base") && !projText.Contains("Revuo.Abstractions"))
                {
                    var insert = @"  <ItemGroup>
    <PackageReference Include=""Revuo.Abstractions"" Version=""1.0.1"" />
    <PackageReference Include=""Revuo.Base"" Version=""1.0.2"" />
    <PackageReference Include=""Revuo.Chat.Client.Base"" Version=""1.0.1"" />
  </ItemGroup>";
                    projText = projText.Replace("</Project>", insert + "</Project>");
                    File.WriteAllText(projFile, projText);
                    result.Log.Add($"Updated project file: {Path.GetFileName(projFile)} (added Revuo package references)");
                }
            }

            // 2) create application class that uses StaticTranslator and registers one control
            var appCs = $@"using Revuo.Chat.Abstraction.Client;
using Revuo.Chat.Base;
using Revuo.Chat.Client.Base.Abstractions;
using Revuo.Chat.Base.I18N;
using System.Threading.Tasks;

namespace {projectName};

public class {projectName}App : BaseThinClientApp
{{
    public {projectName}App() : base(new StaticTranslator(I18N.set))
    {{
    }}

    protected override Task OnInit()
    {{
        // register a simple control
        this.AddControl<HomeControl>();
        return Task.CompletedTask;
    }}
}}
";
            File.WriteAllText(Path.Combine(projectPath, "App.cs"), appCs);
            result.Log.Add("Created App.cs (basic Revuo app)");

            // 3) create translations file (minimal)
            var i18n = $@"using Revuo.Chat.Base.I18N;

namespace {projectName};

public static class I18N
{{
    public static TranslationSet set = new TranslationSet()
    {{
        Translations =
        {{
            [""en-US""] = new Translation()
            {{
                Entries =
                {{
                    [""HELLO_0""] = ""Hello from {projectName}""
                }}
            }}
        }}
    }};
}}
";
            File.WriteAllText(Path.Combine(projectPath, "I18N.cs"), i18n);
            result.Log.Add("Created I18N.cs (translations)");

            // 4) add a simple control (razor)
            var controlRazor = $"@namespace {projectName}\n\n@using Microsoft.AspNetCore.Components.Web\n\n@inherits Revuo.Chat.Client.Base.Abstractions.BasePayloadControlThinClient<NewProjectResponse, {projectName}App>\n\n<div class=\"container mt-4 mb-4\">\n  <h4>Hello from {projectName}</h4>\n  <div class=\"mb-2\">This template includes a simple control and StaticTranslator-based translations.</div>\n</div>\n";
            File.WriteAllText(Path.Combine(projectPath, "HomeControl.razor"), controlRazor);
            result.Log.Add("Created HomeControl.razor");

            result.Percent = 100;
            result.IsCompleted = true;
            return result;
        }
        catch (Exception ex)
        {
            result.WithError(this.Translator, result.Culture, "ERROR_CREATE_REVUOAPP_0", ex.Message);
            return result;
        }
    }
}
