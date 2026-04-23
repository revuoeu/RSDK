using System.Text.Json;
using RSDK.Client.Model;

namespace RSDK.Client.Test;

/// <summary>
/// Plain model tests — no DI or Blazor needed.
/// Covers default values and JSON round-trips for the core RSDK models.
/// </summary>
public class ModelTests
{
    // ── NewProjectResponse ────────────────────────────────────────────────────

    [Fact]
    public void NewProjectResponse_Defaults_AreEmpty()
    {
        var r = new NewProjectResponse();
        Assert.Equal("", r.ProjectPath);
        Assert.Equal("", r.ProjectName);
        Assert.Equal(ProjectType.Unkown, r.ProjectType);
    }

    [Fact]
    public void NewProjectResponse_SurvivesJsonRoundTrip()
    {
        var original = new NewProjectResponse
        {
            ProjectPath = @"C:\Projects\MyApp",
            ProjectName = "MyApp",
            ProjectType = ProjectType.CSharp,
        };

        var json   = JsonSerializer.Serialize(original);
        var loaded = JsonSerializer.Deserialize<NewProjectResponse>(json)!;

        Assert.Equal(original.ProjectPath, loaded.ProjectPath);
        Assert.Equal(original.ProjectName, loaded.ProjectName);
        Assert.Equal(original.ProjectType, loaded.ProjectType);
    }

    // ── ProjectCreateProgress ─────────────────────────────────────────────────

    [Fact]
    public void ProjectCreateProgress_Defaults_AreCorrect()
    {
        var p = new ProjectCreateProgress();
        Assert.Equal(0, p.Percent);
        Assert.Equal("", p.CurrentStep);
        Assert.False(p.IsCompleted);
        Assert.Empty(p.Log);
    }

    [Fact]
    public void ProjectCreateProgress_Log_AccumulatesEntries()
    {
        var p = new ProjectCreateProgress();
        p.Log.Add("Step 1");
        p.Log.Add("Step 2");
        Assert.Equal(2, p.Log.Count);
        Assert.Equal("Step 1", p.Log[0]);
    }

    [Fact]
    public void ProjectCreateProgress_Percent_CanBeUpdated()
    {
        var p = new ProjectCreateProgress { Percent = 42 };
        Assert.Equal(42, p.Percent);
    }

    [Fact]
    public void ProjectCreateProgress_IsCompleted_DefaultsFalse()
    {
        var p = new ProjectCreateProgress();
        Assert.False(p.IsCompleted);
        p.IsCompleted = true;
        Assert.True(p.IsCompleted);
    }

    // ── SdkSettings ───────────────────────────────────────────────────────────

    [Fact]
    public void SdkSettings_DefaultFolder_IsEmpty()
    {
        var s = new SdkSettings();
        Assert.Equal("", s.DefaultNewProjectFolder);
    }

    [Fact]
    public void SdkSettings_SurvivesJsonRoundTrip()
    {
        var original = new SdkSettings
        {
            DefaultNewProjectFolder = @"C:\Projects",
        };

        var json   = JsonSerializer.Serialize(original);
        var loaded = JsonSerializer.Deserialize<SdkSettings>(json)!;

        Assert.Equal(original.DefaultNewProjectFolder, loaded.DefaultNewProjectFolder);
    }
}
