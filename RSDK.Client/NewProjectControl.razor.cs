using System;
using System.IO;
using System.Threading.Tasks;

namespace RSDK.Client
{
    public partial class NewProjectControl
    {
        private string ParentFolder = string.Empty;

        public ProjectType ProjectType = ProjectType.CSharp;

        public SdkSettings Settings { get; private set; }

        public string ComputedProjectPath => Path.Combine(Settings?.DefaultNewProjectFolder ?? string.Empty, Payload?.ProjectName ?? string.Empty);

        protected override async Task OnInitializedAsync()
        {
            this.Settings = await this.RunAction<SdkSettings>(nameof(SDKApp.GetSdkSettings), null);
            if(this.Payload.ProjectName == null)
                this.Payload.ProjectName = "NewProject";

            int counter=0;            
            // check if folder exists
            while (Directory.Exists(this.ComputedProjectPath))
            {
                counter++;
                this.Payload.ProjectName = $"NewProject_{counter}";
            }
        }
    }
}