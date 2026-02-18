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
                    ["ERROR_FOLDER_EXISTS_0"] = "Folder '{0}' already exists. Please choose a different project name or delete the existing folder."
                }
            }
        }
    };
    
}