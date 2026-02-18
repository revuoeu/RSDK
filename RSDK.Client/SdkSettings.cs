using Revuo.Chat.Abstraction.Base;

namespace RSDK.Client;
public class SdkSettings : BasePayloadEntity
{
    internal static readonly string Key = "sdk.settings";

    // Default folder where new projects are created (persisted to device storage)
    public string DefaultNewProjectFolder { get; set; } = string.Empty;
}