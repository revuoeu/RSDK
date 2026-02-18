using Revuo.Chat.Abstraction.Base;


public enum ProjectType
{
    CSharp
}
public class NewProjectResponse : BasePayload
{
    public ProjectType ProjectType { get; set; }

    // Full path (folder) where the new project will be created / opened.
    public string ProjectPath { get; set; } = string.Empty;
}