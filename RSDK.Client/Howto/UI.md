# User Interface


This document explains how to build Blazor/Razor controls for RSDK apps.

## Base class

Controls typically inherit from `BasePayloadControlThinClient<TPayload, TApp>`.

```razor
@namespace RSDK.Client
@inherits Revuo.Chat.Client.Base.Abstractions.BasePayloadControlThinClient<SettingsData, SDKApp>
```

- `TPayload` is the payload shown by the control.
- For simple editors, the payload can be the entity itself.

## General rules

- Load data in app actions, not in the control.
- Treat `Payload` as already populated input for the view.
- Prefer a split `.razor` + `.razor.cs` control.
- Register each control in the app's `OnInit`.
- Use Bootstrap classes for layout and spacing.
- Prefer `mt-4 mb-4` on the top-level container.
- Do not use separate CSS files for controls. Not supported yet.

## Translations

Use the translator exposed by the app or control:

```csharp
var text = Application?.Translator?["SDK.App.ShowSettings"];
```

Some apps expose a shorthand translator property such as `Application?.T?["..."]`. Use the pattern already present in the project.

## Actions from controls

Useful capabilities exposed by the framework:

- `RunAction` to invoke actions
- `ParentFrame.Show(payload)` to navigate
- `DialogService` for dialogs
- `HasActions` to control the automatic action bar

### Automatic action buttons

Automatic bottom action buttons are intended for actions that do not need request arguments.

```csharp
AddAction(GetSettings);
```

If an action requires a request payload, wire the button manually in the markup and call `RunAction`.

```csharp
AddAction<SaveSettingsRequest, SaveSettingsResponse>(SaveSettings);
```

### Hiding the automatic action bar

Override `HasActions` and return `false` when the control owns all button rendering itself.

```csharp
public override bool HasActions => false;
```

Use this for row-level actions, inline editors, or screens where the default bottom bar is not useful.

## Typical pattern

Copy payload values into local fields in `OnInitializedAsync` and bind the UI to those fields.

```csharp
protected override async Task OnInitializedAsync()
{
    if (Payload is not null)
    {
        Name = Payload.Name;
        Items = Payload.Items;
    }

    await base.OnInitializedAsync();
}
```

## Collections

For list screens:

- render add/edit/delete controls inline
- use `RunAction` for mutations
- call `ParentFrame.Show(updatedPayload)` or refresh after save
- keep per-row actions near the row they affect

### Inline add-item pattern

A common approach is a local boolean flag that toggles an inline input form, avoiding the need to navigate away:

```csharp
// in the .razor.cs partial
private bool _adding;
private string _newItemName = "";

private void StartAdd() => _adding = true;
private void CancelAdd() { _adding = false; _newItemName = ""; }

private async Task SaveNewItem()
{
    await RunAction<MyListPayload>("AddItem", new AddItemRequest { Name = _newItemName });
    _adding = false;
    _newItemName = "";
}
```

```razor
@if (_adding)
{
    <div class="input-group mb-3">
        <input class="form-control" @bind="_newItemName"
               placeholder="@Application?.Translator?["UI.NewItem.Placeholder"]" />
        <button class="btn btn-primary" @onclick="SaveNewItem">
            @Application?.Translator?["UI.Add"]
        </button>
        <button class="btn btn-outline-secondary" @onclick="CancelAdd">
            @Application?.Translator?["UI.Cancel"]
        </button>
    </div>
}
else
{
    <button class="btn btn-sm btn-success mb-3" @onclick="StartAdd">
        @Application?.Translator?["UI.Add"]
    </button>
}
```

The corresponding action takes the request and returns an updated payload the parent frame can display.

See `Controls.md` for the full wiring checklist and `Testing.md` for test patterns.
