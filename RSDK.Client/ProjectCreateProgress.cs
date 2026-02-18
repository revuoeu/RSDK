using Revuo.Chat.Abstraction.Base;

namespace RSDK.Client;

public class ProjectCreateProgress : BasePayloadWithError
{
    // progress 0..100
    public int Percent { get; set; } = 0;

    // human readable step name (e.g. "Create folder", "Run dotnet new")
    public string CurrentStep { get; set; } = string.Empty;

    // whether the full flow completed successfully
    public bool IsCompleted { get; set; } = false;


    // textual log / details shown in the UI
    public List<string> Log { get; set; } = new List<string>();

    internal void SetStep(Revuo.Chat.Abstraction.ITranslator translator, string cullture, string format, params object[] args)
    {
        CurrentStep = translator.FormatWithCulture(cullture, format, args);
        Log.Add(CurrentStep);
    }
}