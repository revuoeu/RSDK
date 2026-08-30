using RSDK.Client.Model;

namespace RSDK.Client;

public class ProjectFolder
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";

    public ProjectType Type { get; set; } = ProjectType.Unkown;
}