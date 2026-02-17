using Revuo.Chat.Abstraction.Base;


public enum ProjectType
{
    CSharp
}
public class NewProjectResponse : BasePayload
{
    public ProjectType ProjectType { get; set; }
}