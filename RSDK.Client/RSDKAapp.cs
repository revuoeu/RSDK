using Revuo.Chat.Abstraction.Client;
using Revuo.Chat.Base;
using Revuo.Chat.Client.Base.Abstractions;
using Revuo.Chat.Common;

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
        this.AddControl<NewProjectControl>();

        // SDK settings (default folder for new projects)
        this.AddAction<SdkSettings>(GetSdkSettings);
        this.AddAction<SdkSettings>(SaveSdkSettings);
        this.AddControl<SdkSettingsControl>();

        return Task.CompletedTask;
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
    private async Task<SdkSettings> GetSdkSettings(IThinClientContext context)
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
