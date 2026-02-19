using System;
using System.IO;
using System.Threading.Tasks;

namespace RSDK.Client
{
    public partial class NewProjectControl
    {
        // expose a translator instance for Razor markup (shim)
        protected Revuo.Chat.Abstraction.ITranslator Translator => new Revuo.Chat.Base.I18N.StaticTranslator(I18N.set);

        private string ParentFolder = string.Empty;

        public ProjectType ProjectType = ProjectType.CSharp;

        public SdkSettings Settings { get; private set; }

        public string ComputedProjectPath {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Payload.ProjectName))
                    return string.Empty;

                var path =  Path.Combine(Settings?.DefaultNewProjectFolder ?? string.Empty, Payload?.ProjectName ?? string.Empty);

                Payload.ProjectPath = path;

                return Payload.ProjectPath;
            }
        }
        
        protected override async Task OnInitializedAsync()
        {
            this.Settings = await this.RunAction<SdkSettings>(nameof(SDKApp.GetSdkSettings), null);
            if(string.IsNullOrEmpty(this.Payload!.ProjectName))
                this.Payload.ProjectName = "NewProject";

            int counter=0;

            // check if folder exists
            while (Directory.Exists(this.ComputedProjectPath))
            {
                counter++;
                this.Payload.ProjectName = $"NewProject_{counter}";
            }

            await this.InvokeAsync(StateHasChanged);
        }

    }
}