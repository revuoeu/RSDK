using System.Threading.Tasks;

namespace RSDK.Client
{
    public partial class ListProjectsInFolderControl
    {
        protected override async Task OnInitializedAsync()
        {
            // Ensure we have current listing at start
            await Refresh();
        }

        private async Task Refresh()
        {
            // Invoke the action to retrieve the folder listing
            var result = await RunAction<FolderContent>(nameof(SDKApp.ListProjectsInFolder), null);
            if (result != null)
            {
                Payload = result;
            }
        }
    }
}
