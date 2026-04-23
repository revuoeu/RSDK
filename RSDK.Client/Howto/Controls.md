# Controls Checklist

Use this as the quick checklist when adding a new SDK control.

## Before you start

- Define the payload or entity shown by the control.
- Add an app action that loads the payload.
- Make sure the project uses `Microsoft.NET.Sdk.Razor` if it contains `.razor` files.

## Recommended structure

Create:

- `MyControl.razor`
- `MyControl.razor.cs`

If needed, add `@namespace RSDK.Client` so the app type resolves cleanly in `@inherits`.

## Control rules

- Inherit from `BasePayloadControlThinClient<TPayload, TApp>`.
- Read from `Payload`; do not load app data directly in the control.
- Copy payload values into local fields in `OnInitialized` / `OnInitializedAsync`.
- Use Bootstrap for layout.
- Use `mt-4 mb-4` on the top-level container.
- Register the control in `OnInit` with `AddControl<YourControl>()`.

## Actions and navigation

- Use `RunAction<TResponse>(...)` for user-triggered operations.
- Use `ParentFrame.Show(...)` for navigation to another payload-backed control.
- Set `HasActions => false` only when you do not want the automatic bottom action bar.

## Minimal example

```razor
@namespace RSDK.Client
@inherits Revuo.Chat.Client.Base.Abstractions.BasePayloadControlThinClient<SdkSettings, SDKApp>

<div class="container mt-4 mb-4">
    <h3>@Application?.Translator?["SDK.App.Settings"]</h3>
    <input class="form-control" @bind="Name" />
</div>
```

```csharp
public partial class SdkSettingsControl
{
    private string? Name;

    protected override async Task OnInitializedAsync()
    {
        Name = Payload?.Name;
        await base.OnInitializedAsync();
    }
}
```

## Final checklist

- payload type exists
- action returns populated payload
- control is registered
- translations are added
- `HasActions` is set only if needed
- unit tests are added for action logic and control behavior

See `Testing.md` for the recommended test stack.
