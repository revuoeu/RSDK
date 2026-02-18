using Revuo.Chat.Abstraction.Base;


namespace RSDK.Client;
public enum ProjectType
{
    CSharp
}

public class NewProjectResponse : BasePayload
{
    public ProjectType ProjectType { get; set; }

    // Full path (folder) where the new project will be created / opened.
    public string ProjectPath { get; set; } = string.Empty;

    public string ProjectName { get; set; } = string.Empty;
}