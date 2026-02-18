using Revuo.Chat.Abstraction.Base;

public class SdkSettings : BasePayload
{
    // Default folder where new projects are created (persisted to device storage)
    public string DefaultNewProjectFolder { get; set; } = string.Empty;
}