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

public partial class SDKApp : BaseThinClientApp
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
        this.AddAction<ProjectCreateProgress, ProjectCreateProgress>(CreateNewProject_CreateReadme);
        this.AddAction<ProjectCreateProgress, ProjectCreateProgress>(CreateNewProject_CreateSolution);
        this.AddAction<ProjectCreateProgress, ProjectCreateProgress>(CreateNewProject_CreateGitIgnore);
        this.AddAction<ProjectCreateProgress, ProjectCreateProgress>(CreateNewProject_CreateRevuoApp);
        this.AddControl<NewProjectControl>();

        // SDK settings (default folder for new projects)
        this.AddAction<SdkSettings>(GetSdkSettings);
        this.AddAction<SdkSettings>(SaveSdkSettings);
        this.AddControl<SdkSettingsControl>();
        this.AddControl<ProjectCreateProgressControl>();

        return Task.CompletedTask;
    }

    // CreateNewProject* methods moved to `SDKApp.CreateNewProject.cs` (partial)



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
