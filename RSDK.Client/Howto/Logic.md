# Application Logic (C#)

This section covers the non-UI C# code in the application: the central app class, actions, storage, and interaction via `IThinClientContext`.

## Application class (e.g. YourApp)

Your application's central class subclasses `BaseThinClientApp` and registers actions/controls in `OnInit`.

> **Statelessness**: application actions should not rely on mutable state stored on the app instance; instead, they fetch any required data from storage or parameters. Helper methods that create service clients (e.g. YouTubeClient) should also be stateless and create new instances as needed.

```csharp
public class YourApp : BaseThinClientApp
{
    public YourApp() : base(new StaticTranslator(I18N.set)) { }

    protected override async Task OnInit()
    {
        // register user actions
        AddAction(ShowChannels);
        AddAction<RefreshChannelsRequest>(RefreshAll);
        AddAction<LoadPlaylistRequest, ShowPlaylistRequest>(ShowPlaylist);
        AddAction<ShowVideoRequest, ShowVideoRequest>(ShowVideo);
        AddAction(GetSettings);                // no-argument action
        AddAction<SaveSettingsRequest, SaveSettingsRequest>(SaveSettings);

        // register UI controls
        AddControl<ChannelListView>();
        AddControl<PlaylistView>();
        AddControl<Player>();
        AddControl<SettingsView>();

        await base.OnInit();
    }
}
```

Actions are normal methods returning payloads or entities; they receive an `IThinClientContext` which exposes storage, translation, and other framework services.

> **Translation keys for actions** usually mirror the method's fully qualified name.  For example, the `GetSettings` action defined on `MediaApp` can be translated using the key `Media.MediaApp.GetSettings`.

## Storage Patterns

Use `ctx.DeviceStorage` and `ctx.ApplicationStorage` for persistent data. Device storage is local to the current device.

Singleton-type entities should set a fixed `Id` property when stored.

```csharp
class SettingsData : BaseEntity
{
    public static string Key = "app.settings";
    public string YouTubeApiKey { get; set; }
}

var stored = await ctx.DeviceStorage.Get<SettingsData>(SettingsData.Key);
//storing
req.Id = SettingsData.Key;
await ctx.DeviceStorage.Store(req);
```


## Running Actions

Within actions or controls you can call:

```csharp
var resp = await context.RunAction<TResponse>("ActionName", payload);
```

Actions without request payloads are invoked with `null`.

```csharp
var response = await context.RunAction<MyResponse>("GetContacts", null);
```

## Error handling

If an action throws an exception, the framework catches it and handles it according to the Revuo error flow (exposed in the UI frame and developer console). 

For more structured handling, preferred approach is to return `BasePayloadWithErrors<T>` and include localized error information. This allows you to use translated messages in the UI and avoid throwing for standard business errors.

Example payload with errors:

```csharp
public class SaveContactResponse : BasePayloadWithErrors<Contact>
{
}
```

Then in UI, inspect `Payload?.Errors` and show localized errors to the user.

## Stateless YouTube Client Helper

```csharp
public async Task<IYouTubeClient> GetYouTubeClient(IThinClientContext ctx)
{
    var settings = await GetSettings(ctx);
    if (!string.IsNullOrEmpty(settings.YouTubeApiKey))
        return new YouTubeClient(apiKey: settings.YouTubeApiKey);
    return new YouTubeClient();
}
```

This method fetches the current API key from storage each call and instantiates a fresh client, keeping the app stateless.

---

## Checking Authentication State

When you need to determine whether a user is signed in, inspect `context.User.Id`. If the value is **not null** and **not empty**, the user is authenticated; otherwise the caller should throw a `NotAuthenticated` exception. This simple pattern centralizes sign‑in checks in actions or helpers.

```csharp
if (string.IsNullOrEmpty(context.User.Id))
{
    throw new NotAuthenticated();
}
// proceed with authenticated logic
```

## Guidelines
- Suggested place to put model files is in folder Models