using Revuo.Chat.Abstraction.Base;
using Revuo.Chat.Abstraction.Client;
using Revuo.Chat.Abstraction.Extensions;
using Revuo.Chat.Base;
using Revuo.Chat.Client.Base.Abstractions;
using Revuo.Chat.Common;
using Revuo.Chat.Abstraction.Extensions;

namespace RSDK.Client;

public class SDKApp : BaseThinClientApp
{
    public SDKApp() : base(null!)
    {
    }
 
    protected override Task OnInit()
    {
        // register existing action + control
        this.AddAction<NewProjectResponse>(NewProject);
        this.AddAction<NewProjectResponse>(CreateNewProject);
        this.AddAction<NewProjectResponse, ProjectCreateProgress>(CreateNewProject_CreateFolder);
        this.AddControl<NewProjectControl>();

        // SDK settings (default folder for new projects)
        this.AddAction<SdkSettings>(GetSdkSettings);
        this.AddAction<SdkSettings>(SaveSdkSettings);
        this.AddControl<SdkSettingsControl>();

        return Task.CompletedTask;
    }

    private async Task CreateNewProject(IThinClientContext context, NewProjectResponse response)
    {
        
        //1. create folder
        //2. run dotnet new to create classlib for razor framework 10
        //3. copy howto's
        //4. create specific files:
        // a. github pipilines
        // b. vscode launch and tasks
        // c. revuo files to run workflows for installations and installation
        //5. create some readme
        //6. create solution
        //7. crwate git ignore
        
        // all those actions above can be separte actions that are run in sequence, 
        // and report progress to the user via user control of creations. 
        // This allows for better error handling and user feedback.

        var result = await context.RunAction("RSDK.Client.SDKApp", nameof(SDKApp.CreateNewProject_CreateFolder), response) as ProjectCreateProgress;
        result!.ThrowIfError(this.Translator);

        throw new NotImplementedException();
    }

    private Task<ProjectCreateProgress> CreateNewProject_CreateFolder(IThinClientContext context, NewProjectResponse response)
    {
        var result = new ProjectCreateProgress();
        // if folder exists than fail with error message "Folder already exists"
        if (Directory.Exists(response.ProjectPath))
        {
            result.WithError(
                this.Translator,
                response, 
                "ERROR_FOLDER_EXISTS_0", response.ProjectPath);
        }
        return Task.FromResult(result);
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

public class ProjectCreateProgress : BasePayloadWithError
{
}