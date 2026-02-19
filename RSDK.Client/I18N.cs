using Revuo.Chat.Base.I18N;

namespace RSDK.Client;

public static class I18N
{

    public static TranslationSet set = new TranslationSet()
    {
        Translations =
        {
            ["en-US"] = new Translation()
            {
                Entries =
                {
                    ["0_CREATING_FOLDER_1"] = "{0} Creating project folder at '{1}'...",
                    ["CreateNewProject_CreateFolder"] = "Creating project folder",
                    ["CreateNewProject_DotnetNew"] = "Running 'dotnet new' to scaffold the project...",
                    ["CreateNewProject_CopyHowtos"] = "Copying how‑to files...",
                    ["CreateNewProject_CreateSpecificFiles"] = "Creating project-specific files...",
                    ["CreateNewProject_CreateReadme"] = "Creating README...",
                    ["CreateNewProject_CreateSolution"] = "Creating solution and adding projects...",
                    ["CreateNewProject_CreateGitIgnore"] = "Creating .gitignore...",
                    ["CreateNewProject_CreateRevuoApp"] = "Adding Revuo app files...",
                    ["RSDK.Client.SDKApp"] = "Revuo Software Development Kit",                    ["RSDK.Client.SDKApp.NewProject"] = "New project",
                    ["RSDK.Client.SDKApp.CreateNewProject"] = "Create new project",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateFolder"] = "Creating project folder",
                    ["RSDK.Client.SDKApp.CreateNewProject_DotnetNew"] = "Scaffolding project (dotnet new)",
                    ["RSDK.Client.SDKApp.CreateNewProject_CopyHowtos"] = "Copying how‑to files...",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateSpecificFiles"] = "Creating project-specific files...",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateReadme"] = "Creating README...",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateSolution"] = "Creating solution and adding projects...",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateGitIgnore"] = "Creating .gitignore...",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateRevuoApp"] = "Adding Revuo app files...",
                    ["RSDK.Client.SDKApp.GetSdkSettings"] = "Load SDK settings",
                    ["RSDK.Client.SDKApp.SaveSdkSettings"] = "Save SDK settings",

                    // NewProject UI (labels, help text)
                    ["RSDK.Client.NewProjectControl.ProjectType"] = "Project type",
                    ["RSDK.Client.NewProjectControl.ParentFolder"] = "Parent folder",
                    ["RSDK.Client.NewProjectControl.ProjectName"] = "Project name",
                    ["RSDK.Client.NewProjectControl.NewProjectFolder"] = "New project folder",
                    ["RSDK.Client.NewProjectControl.FolderHelp"] = "A new folder with this name will be created under the parent folder.",
                    ["RSDK.Client.NewProjectControl.ParentFolderNote"] = "Parent folder shown is the SDK default (change via SDK Settings control).",

                    // SDK settings UI
                    ["RSDK.Client.SdkSettingsControl.Title"] = "SDK Settings",
                    ["RSDK.Client.SdkSettingsControl.DefaultNewProjectFolder"] = "Default new project folder",
                    ["RSDK.Client.SdkSettingsControl.HelpText"] = "A new project will be created in this folder by default (can be overridden in New Project control).",

                    // Project creation UI
                    ["RSDK.Client.ProjectCreateProgressControl.Title"] = "Creating project",
                    ["RSDK.Client.ProjectCreateProgressControl.Completed"] = "Project creation completed successfully.",
                    ["RSDK.Client.ProjectCreateProgressControl.Details"] = "Details",

                    ["ERROR_FOLDER_EXISTS_0"] = "Folder '{0}' already exists. Please choose a different project name or delete the existing folder.",
                    ["ERROR_DOTNET_NEW_FAILED_0"] = "'dotnet new' failed: {0}",
                    ["ERROR_EXCEPTION_0"] = "Unexpected error: {0}",
                    ["ERROR_COPY_HOWTO_0"] = "Failed to copy how‑to files: {0}",
                    ["ERROR_CREATE_FILES_0"] = "Failed to create project files: {0}",
                    ["ERROR_CREATE_README_0"] = "Failed to create README: {0}",
                    ["ERROR_CREATE_SOLUTION_0"] = "Failed to create solution: {0}",
                    ["ERROR_ADD_PROJECT_TO_SOLUTION_0"] = "Failed to add project to solution: {0}",
                    ["ERROR_CREATE_GITIGNORE_0"] = "Failed to create .gitignore: {0}",
                    ["ERROR_CREATE_REVUOAPP_0"] = "Failed to create Revuo app: {0}"
                }
            },

            ["pl-PL"] = new Translation()
            {
                Entries =
                {
                    ["0_CREATING_FOLDER_1"] = "{0} Tworzenie folderu projektu w '{1}'...",
                    ["CreateNewProject_CreateFolder"] = "Tworzenie folderu projektu",
                    ["CreateNewProject_DotnetNew"] = "Uruchamianie 'dotnet new' w celu utworzenia projektu...",
                    ["CreateNewProject_CopyHowtos"] = "Kopiowanie plików instrukcji (how‑to)...",
                    ["CreateNewProject_CreateSpecificFiles"] = "Tworzenie plików specyficznych dla projektu...",
                    ["CreateNewProject_CreateReadme"] = "Tworzenie pliku README...",
                    ["CreateNewProject_CreateSolution"] = "Tworzenie rozwiązania i dodawanie projektów...",
                    ["CreateNewProject_CreateGitIgnore"] = "Tworzenie pliku .gitignore...",
                    ["CreateNewProject_CreateRevuoApp"] = "Dodawanie plików aplikacji Revuo...",

                    ["RSDK.Client.SDKApp"] = "Zestaw narzędzi programistycznych Revuo",
                    ["RSDK.Client.SDKApp.NewProject"] = "Nowy projekt",
                    ["RSDK.Client.SDKApp.CreateNewProject"] = "Utwórz nowy projekt",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateFolder"] = "Tworzenie folderu projektu",
                    ["RSDK.Client.SDKApp.CreateNewProject_DotnetNew"] = "Uruchamianie 'dotnet new' w celu utworzenia projektu..."
                    ["RSDK.Client.SDKApp.CreateNewProject_CopyHowtos"] = "Kopiowanie plików instrukcji (how‑to)...",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateSpecificFiles"] = "Tworzenie plików specyficznych dla projektu...",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateReadme"] = "Tworzenie pliku README...",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateSolution"] = "Tworzenie rozwiązania i dodawanie projektów...",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateGitIgnore"] = "Tworzenie pliku .gitignore...",
                    ["RSDK.Client.SDKApp.CreateNewProject_CreateRevuoApp"] = "Dodawanie plików aplikacji Revuo...",
                    ["RSDK.Client.SDKApp.GetSdkSettings"] = "Wczytaj ustawienia SDK",
                    ["RSDK.Client.SDKApp.SaveSdkSettings"] = "Zapisz ustawienia SDK",

                    // NewProject UI (labels, help text)
                    ["RSDK.Client.NewProjectControl.ProjectType"] = "Typ projektu",
                    ["RSDK.Client.NewProjectControl.ParentFolder"] = "Folder nadrzędny",
                    ["RSDK.Client.NewProjectControl.ProjectName"] = "Nazwa projektu",
                    ["RSDK.Client.NewProjectControl.NewProjectFolder"] = "Folder nowego projektu",
                    ["RSDK.Client.NewProjectControl.FolderHelp"] = "Nowy folder o tej nazwie zostanie utworzony w folderze nadrzędnym.",
                    ["RSDK.Client.NewProjectControl.ParentFolderNote"] = "Wyświetlany folder nadrzędny to domyślny folder SDK (zmień w panelu Ustawienia SDK).",

                    // SDK settings UI
                    ["RSDK.Client.SdkSettingsControl.Title"] = "Ustawienia SDK",
                    ["RSDK.Client.SdkSettingsControl.DefaultNewProjectFolder"] = "Domyślny folder dla nowych projektów",
                    ["RSDK.Client.SdkSettingsControl.HelpText"] = "Nowy projekt zostanie utworzony w tym folderze domyślnie (można zmienić w kontrolce Nowy projekt).",

                    // Project creation UI
                    ["RSDK.Client.ProjectCreateProgressControl.Title"] = "Tworzenie projektu",
                    ["RSDK.Client.ProjectCreateProgressControl.Completed"] = "Tworzenie projektu zakończone pomyślnie.",
                    ["RSDK.Client.ProjectCreateProgressControl.Details"] = "Szczegóły",

                    ["ERROR_FOLDER_EXISTS_0"] = "Folder '{0}' już istnieje. Wybierz inną nazwę projektu lub usuń istniejący folder.",
                    ["ERROR_DOTNET_NEW_FAILED_0"] = "'dotnet new' nie powiódł się: {0}",
                    ["ERROR_EXCEPTION_0"] = "Nieoczekiwany błąd: {0}",
                    ["ERROR_COPY_HOWTO_0"] = "Nie udało się skopiować plików instrukcji: {0}",
                    ["ERROR_CREATE_FILES_0"] = "Nie udało się utworzyć plików projektu: {0}",
                    ["ERROR_CREATE_README_0"] = "Nie udało się utworzyć README: {0}",
                    ["ERROR_CREATE_SOLUTION_0"] = "Nie udało się utworzyć rozwiązania: {0}",
                    ["ERROR_ADD_PROJECT_TO_SOLUTION_0"] = "Nie udało się dodać projektu do rozwiązania: {0}",
                    ["ERROR_CREATE_GITIGNORE_0"] = "Nie udało się utworzyć .gitignore: {0}",
                    ["ERROR_CREATE_REVUOAPP_0"] = "Nie udało się utworzyć aplikacji Revuo: {0}"
                }
            }
        }
    };
    
}