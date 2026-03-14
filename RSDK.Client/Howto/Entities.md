# Payloads & Entities

- **Payloads**: use `BasePayload<T>` for request/response messages.
  All action result or paramters implements IPayload. There are base class helper to use: BasePayload<T>, BasePayloadWithErrors<T>.

- **UI Entities**: inherit from `IPayload` when they need to flow through the Revuo framework.
- **Storable Entities**: enetities that can be stored in `DeviceStorage` or `ApplicationStorage` should inherit from `IEntity` which includes an `Id` property. There are helper base classes like `BasePayloadEntity` that combine both concepts for convenience.


```csharp
public class ShowChannelsResponse : BasePayload<List<Channel>> { }
public class SettingsData : BasePayloadEntity { public string YouTubeApiKey { get; set; } }
```

Use the *entity-edit* pattern (no request payload) for simple screens like settings:

```csharp
AddAction(GetSettings);
public async Task<SettingsData> GetSettings(IThinClientContext ctx) { ... }
```
