using System;
using System.IO;
using System.Threading.Tasks;

namespace RSDK.Client
{
    public partial class NewProjectControl
    {
        private string ParentFolder = string.Empty;
        private string ProjectName = "NewProject";

        public ProjectType ProjectType = ProjectType.CSharp;

        public SdkSettings Settings { get; private set; }

        public string ComputedProjectPath => Path.Combine(Settings?.DefaultNewProjectFolder ?? string.Empty, ProjectName);

        protected override async Task OnInitializedAsync()
        {
            this.Settings = await this.RunAction<SdkSettings>(nameof(SDKApp.GetSdkSettings), null);
            
        }


    }
}