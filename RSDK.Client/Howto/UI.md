# User Interface (Controls)

This section explains how to build Blazor/Razor controls within the Revuo framework.

## Base Classes

Controls usually inherit from `BasePayloadControlThinClient<TPayload, TApp>`. `TPayload` corresponds to the type returned by the action that navigates to the control.

```razor
@inherits Revuo.Chat.Client.Base.Abstractions.BasePayloadControlThinClient<SettingsData, YourApp>
```

For simple entity editors the payload type can be the entity itself rather than a wrapper.

## Internationalization in Controls

Use `Application?.Translator?["key"]` to look up translations. Translation keys are managed in `I18N.set`.

## Using Payloads

Controls will have typed Payload property populated. No need to load it. It will be availble in this.Payload

## Framework Capabilities

- **RunAction**: invoke actions from within a control.
- **ParentFrame.Show(payload)**: navigate to another control with a payload.
- **HasActions**: override to control whether the automatic action bar is rendered (see below).
- **DialogService**: injected service for displaying dialogs.

### Automatic action buttons

Revuo can automatically render action buttons in the action bar at the bottom of a control — **but only for actions that take no arguments** (registered with `AddAction(method)`, no `TReq` type parameter).

If an action requires a request argument (`AddAction<TReq, TResp>(method)`), the framework has no way to know what value to pass, so **no automatic button is shown**. Those actions must be wired manually in the control's markup.

Example — automatic button (no argument):
```csharp
AddAction(GetCampaigns);  // button appears automatically
```

Example — no automatic button (has argument):
```csharp
AddAction<CampaignActionRequest, CampaignListResponse>(StartCampaign);  // must wire manually
```

### Controlling the action bar with `HasActions`

The base class exposes a virtual `HasActions` property that defaults to `true`. Revuo uses it to decide whether to render the automatic action bar at all.

**Default (action bar visible):**  
Do nothing — the framework renders automatic buttons for any no-argument actions whose return type matches the control's payload type.

**Suppress the action bar entirely:**  
Override and return `false` when the control manages all its own buttons inline (e.g. per-row actions in a list view):

```csharp
public override bool HasActions => false;
```

Use this when:
- All actions require arguments (so no auto-buttons would appear anyway) and you don't want the empty action bar rendered.
- The control has buttons on individual rows (edit, start, stop, poll…) rather than a single global action.
- You want full control over button placement and visibility logic.

`CampaignListView` is an example: it overrides `HasActions => false` because all its actions take a `CampaignActionRequest` (the campaign ID) and are rendered inline per row.

### Adding new items (e.g. channels)

For collection editors you will often want a button or form at the top of the view to create a new element.  `ChannelListView` demonstrates one approach:

```razor
@if (addingChannel)
{
    <div class="input-group mb-3">
        <input class="form-control" @bind="newChannelName" placeholder="@Application?.Translator["UI.AddChannel.Placeholder"]" />
        <button class="btn btn-primary" @onclick="SaveNewChannel">@Application?.Translator["UI.AddChannel"]</button>
        <button class="btn btn-outline-secondary" @onclick="CancelAddChannel">@Application?.Translator["UI.Cancel"]</button>
    </div>
}
else
{
    <button class="btn btn-sm btn-success mb-3" @onclick="StartAddChannel">
        @Application?.Translator["UI.AddChannel"]
    </button>
}
```

The action itself is registered in the app (`AddAction<AddChannelRequest, ShowChannelsResponse>(AddChannel)`) and invoked via `RunAction`, returning an updated channel list which the parent frame can display.  Translate the button text using the usual `Application?.Translator["…"]` key lookup.

Layout and styling use Bootstrap classes (`container`, `form-control`, `btn`, etc.).

Controls are registered in your application class's `OnInit` method.

### Editing collections

When a control renders a list (e.g. channels containing playlists) you often want per-item operations such as rename, delete, or add/remove sub-items. Render extra buttons or icons next to each element and call corresponding actions with `RunAction`.

### Guidelines
Prefered way to create control is to split markup and codebehind into separeted razor.cs files.