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
- **HasActions**: override to hide automatic action bar.
- **DialogService**: injected service for displaying dialogs.

### Automatic action buttons

When a control's payload type matches the request type of a registered action, Revuo will automatically display that action as a button in the action bar at the bottom.  For example, if the control is `BasePayloadControlThinClient<ShowChannelsResponse, YourApp>` and the app registers

```csharp
AddAction<ShowChannelsResponse, ShowChannelsResponse>(SaveChannels);
```

then the UI will render a "SaveChannels" button without you having to add it manually in the markup.  This is the preferred pattern for operations that operate on the current entity (such as saving a list or editing an item).

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