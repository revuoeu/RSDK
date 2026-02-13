using Revuo.Chat.Abstraction.Base;

public class NewProjectResponse : BasePayload
{
    public string ProjectType { get; set; } = string.Empty;
}