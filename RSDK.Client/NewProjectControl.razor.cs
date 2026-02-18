using System;
using System.IO;
using System.Threading.Tasks;

namespace RSDK.Client
{
    public partial class NewProjectControl
    {
        private string ProjectTypeText = "—";
        private string ParentFolder = string.Empty;
        private string ProjectName = "NewProject";
        private string ProjectPathText = "—";

        private string CombinedPath => string.IsNullOrWhiteSpace(ParentFolder)
            ? ProjectName
            : Path.Combine(ParentFolder, ProjectName ?? string.Empty);

        protected override async Task OnInitializedAsync()
        {
            if (Payload is not null)
            {
                ProjectTypeText = Payload.ProjectType.ToString();
                ParentFolder = string.IsNullOrWhiteSpace(Payload.ProjectPath)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    : Payload.ProjectPath;
                ProjectPathText = ParentFolder;
            }

            await base.OnInitializedAsync();
        }

        private void CreatePreview()
        {
            // Show computed folder path in the UI; actual folder creation is platform-specific and not performed here.
            ProjectPathText = CombinedPath;
        }
    }
}