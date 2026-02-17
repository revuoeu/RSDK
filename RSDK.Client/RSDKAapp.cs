using Revuo.Chat.Abstraction.Client;
using Revuo.Chat.Base;

namespace RSDK.Client;

public class SDKApp : BaseThinClientApp
{
    public SDKApp() : base(null!)
    {
    }
 
    protected override Task OnInit()
    {
        this.AddAction<NewProjectResponse>(NewProject);

        return Task.CompletedTask;
    }

    private async Task<NewProjectResponse> NewProject(IThinClientContext context)
    {
        return new NewProjectResponse
        {
            ProjectType = ProjectType.CSharp,
        };
    }
}
