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
using Revuo.Chat.Abstraction;
using RSDK.Client.Model;
using Revuo.Chat.Abstraction.Base;

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
        this.AddAction<ProjectCreateProgress, ProjectCreateProgress>(CreateNewProject_CreateTestProject);
        this.AddControl<NewProjectControl>();

        // SDK settings (default folder for new projects)
        this.AddAction<SdkSettings>(GetSdkSettings);
        this.AddAction<SdkSettings>(SaveSdkSettings);
        this.AddAction<FolderContent>(ListProjectsInFolder);


        this.AddControl<SdkSettingsControl>();
        this.AddControl<ProjectCreateProgressControl>();
        this.AddControl<ListProjectsInFolderControl>();

        this.AddAction(MyApplications);

        return Task.CompletedTask;
    }

    public async Task<FolderContent> ListProjectsInFolder(IThinClientContext context)
    {
        // get path from settings
        // list all folder in that path
        var settings = await GetSdkSettings(context);
        var path = settings.DefaultNewProjectFolder;
        var folders = new List<ProjectFolder>();
        if (Directory.Exists(path))
        {
            var dirs = Directory.GetDirectories(path);

            foreach (var d in dirs)
            {
                var project = new ProjectFolder
                {
                    Name = Path.GetFileName(d),
                    Path = d
                };

                // check if this is a dotnet project (contains .csproj or .sln file)
                var files = Directory.GetFiles(d);
                if (files.Any(f => f.EndsWith(".csproj") || f.EndsWith(".sln")))
                    project.Type = ProjectType.CSharp;
                
                folders.Add(project);
            }
        }

        return new FolderContent
        {
            D = folders
        };
    }

    private async Task<ApplicationList> MyApplications(IThinClientContext context)
    {
        // let's ask server for the list of applications (projects) we have created
        // we can go trhough installer app so we will run local actions agains installer
        // it should be able to mange projects and approvals and evrything

        // let's maintain complete list of all application in github
        // those will have link to the github projects

        // this list will have ID if it's registered in app manager
        // no id then not public
        // 
        // if we add app to this list we can install and share with others
        // we can apply for approval (with version of the artifact)
        // we will handle storage types registartion
        // request for starage can be separted
        // 

        var list = await context.RunAction("AppManager.Client.AppManagerApp", "ListApps", null);

        return null;
    }

    // CreateNewProject* methods moved to `SDKApp.CreateNewProject.cs` (partial)



    private async Task<NewProjectResponse> NewProject(IThinClientContext context)
    {
        var response = new NewProjectResponse
        {
            ProjectType = ProjectType.CSharp,
        };

        // attempt to read stored default folder from device storage (key: sdk.defaultNewProjectFolder)
        var settings = await GetSdkSettings(context);
        if (!string.IsNullOrWhiteSpace(settings?.DefaultNewProjectFolder))
            response.ProjectPath = settings!.DefaultNewProjectFolder;
    

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

public class FolderContent : BasePayload<List<ProjectFolder>>
{
}

public class ProjectFolder
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";

    public ProjectType Type { get; set; } = ProjectType.Unkown;
}