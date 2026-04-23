using FakeItEasy;
using Revuo.Chat.Abstraction;
using Revuo.Chat.Abstraction.Client;
using Revuo.Chat.Abstraction.Models;
using Revuo.Chat.Base;
using RSDK.Client.Model;

namespace RSDK.Client.Test;

/// <summary>
/// Action-level tests for SDKApp using in-memory fake storage.
/// Follows the same pattern as Kriok's CampaignSaveLoadCycleTests.
/// </summary>
public class SdkAppActionTests
{
    private SdkSettings? _storedSettings;
    private readonly SDKApp _app = new();

    static SdkAppActionTests()
    {
        TypeResolver.Init();
        var idFactory = A.Fake<IIdFactory>();
        A.CallTo(() => idFactory.GenerateId()).ReturnsLazily(() => Guid.NewGuid().ToString());
        IdProvider.Init(idFactory);
    }

    private IThinClientContext MakeContext()
    {
        var deviceStorage = A.Fake<IUserStorage>();

        A.CallTo(() => deviceStorage.Get<SdkSettings>(A<string>._))
            .ReturnsLazily(_ => Task.FromResult(_storedSettings));

        A.CallTo(() => deviceStorage.Store(A<SdkSettings>._))
            .Invokes(call => _storedSettings = call.Arguments.Get<SdkSettings>(0));

        var ctx = A.Fake<IThinClientContext>();
        A.CallTo(() => ctx.DeviceStorage).Returns(deviceStorage);
        A.CallTo(() => ctx.User).Returns(new UserReference { Id = "test-user" });
        return ctx;
    }

    // ── GetSdkSettings ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetSdkSettings_WhenNothingStored_ReturnsFallbackFolder()
    {
        var ctx = MakeContext();

        var settings = await _app.GetSdkSettings(ctx);

        // storage is empty; the app should fall back to MyDocuments (non-empty)
        Assert.False(string.IsNullOrWhiteSpace(settings.DefaultNewProjectFolder));
    }

    [Fact]
    public async Task GetSdkSettings_WhenStored_ReturnsStoredFolder()
    {
        _storedSettings = new SdkSettings { DefaultNewProjectFolder = @"C:\Projects" };
        var ctx = MakeContext();

        var settings = await _app.GetSdkSettings(ctx);

        Assert.Equal(@"C:\Projects", settings.DefaultNewProjectFolder);
    }

    // ── ListProjectsInFolder ─────────────────────────────────────────────────

    [Fact]
    public async Task ListProjectsInFolder_WhenFolderDoesNotExist_ReturnsEmptyList()
    {
        _storedSettings = new SdkSettings { DefaultNewProjectFolder = @"C:\DoesNotExist_XYZ_12345" };
        var ctx = MakeContext();

        var result = await _app.ListProjectsInFolder(ctx);

        Assert.NotNull(result);
        Assert.Empty(result.D ?? []);
    }
}
