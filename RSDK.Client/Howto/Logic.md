# Application Logic

This document covers the app class, actions, storage usage, authentication checks, and error handling.

## App class

Your main app class typically derives from `BaseThinClientApp` and registers actions and controls in `OnInit`.

```csharp
public class SDKApp : BaseThinClientApp
{
    public SDKApp() : base(new StaticTranslator(I18N.set)) { }

    protected override async Task OnInit()
    {
        AddAction(GetSettings);
        AddAction<SaveSettingsRequest, SaveSettingsResponse>(SaveSettings);

        AddControl<SettingsControl>();

        await base.OnInit();
    }
}
```

## Design rules

- Keep actions stateless.
- Do not rely on mutable app-instance state.
- Fetch data from storage, the request, or a service created for the call.
- Load UI data in actions and pass it to controls through payloads.

## Calling actions

From actions or controls you can run another action:

```csharp
var response = await context.RunAction<MyResponse>("ActionName", request);
```

For parameterless actions, pass `null` if the framework signature requires it.

## Authentication checks

When an operation requires a signed-in user, validate `context.User.Id`.

```csharp
if (string.IsNullOrEmpty(context.User?.Id))
{
    throw new NotAuthenticated();
}
```

Put this check in a helper when several actions share the same rule.

## Error handling

Throw framework exceptions such as `NotAuthenticated` for infrastructure or access failures.

For expected business errors, prefer returning `BasePayloadWithErrors<T>` so the UI can show translated messages without using exceptions for normal validation flow.

```csharp
public class SaveFooResponse : BasePayloadWithErrors<Foo>
{
}
```

## Suggested layout

- app class in the client project root
- models and payloads in `Models`
- storage helper in a shared helper file
- control logic in `.razor.cs`

See `Storage.md` for persistence rules and `Entities.md` for payload/entity patterns.
