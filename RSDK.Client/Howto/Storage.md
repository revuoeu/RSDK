# Storage Access

This document describes the recommended pattern for accessing storage in a Revuo app.

## Storage choice

Use `ctx.ApplicationStorage` for user-scoped persistence (requires authentication).
Use `ctx.DeviceStorage` only for local, device-scoped data that doesn't need a logged-in user.

## Recommended helper

Centralize storage access through a single helper so the source can be changed in one place:

```csharp
public static class StorageHelper
{
    // Returns device storage. Add auth check and switch to ApplicationStorage when needed.
    public static IDeviceStorage GetStorage(IThinClientContext ctx)
        => ctx.DeviceStorage;
}
```

## Usage

Use `Load` to read and `Save` to write:

```csharp
var storage = StorageHelper.GetStorage(ctx);
var store = await storage.Get<MyStore>(MyStore.Key) ?? new MyStore();

store.Id = MyStore.Key;
await storage.Store(store);
```

The `?? new MyStore()` guard handles the first run when nothing is stored yet.

## Entity Registration

Every store type persisted via `storage.Store(...)` must be registered in `App.OnInit` so the framework knows how to serialize/deserialize it:

```csharp
RegisterEntity<ContactStore>();
RegisterEntity<SenderStore>();
RegisterEntity<CampaignStore>();
RegisterEntity<CampaignLogStore>();
```

If a store is written but not registered, data may fail to persist or load correctly.
