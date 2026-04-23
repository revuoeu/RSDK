# Payloads and Entities

Use payloads for transport through the framework and entities for persistent records.

## Payloads

Use payloads for action requests and responses.

- `BasePayload<T>` for normal payloads
- `BasePayloadWithErrors<T>` when the response may include validation or business errors

```csharp
public class ShowChannelsResponse : BasePayload<List<Channel>> { }
```

## Entities

- Use `IEntity` for storable records.
- Use `BasePayloadEntity` when the same type needs both payload behavior and an `Id`.

```csharp
public class SettingsData : BasePayloadEntity
{
    public string? ApiKey { get; set; }
}
```

## IDs

Use `IdProvider.Get()` for new IDs instead of `Guid.NewGuid().ToString()`.

```csharp
var contact = new Contact
{
    Id = IdProvider.Get()
};
```

For singleton-like records, use a fixed key instead of generating a new ID every time.

## Simple editor pattern

For settings and similar screens, a parameterless action that returns the entity is often enough.

```csharp
AddAction(GetSettings);

public async Task<SettingsData> GetSettings(IThinClientContext context)
{
    // load and return the entity
}
```

That lets the control work directly with the returned entity payload.
