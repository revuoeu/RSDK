# Storage

**Sources:** `RSDK.Client\Howto\Storage.md`, `DalleR\Howto\Storage.md`

This document describes the recommended storage pattern in RSDK apps.

## Choose the right storage

- Use `context.ApplicationStorage` for authenticated, user-scoped data.
- Use `context.DeviceStorage` for local device data that does not depend on a signed-in user.

Default to `ApplicationStorage` unless the data is intentionally local-only.

## Centralize access

Wrap storage access in one helper so auth checks and future changes stay in one place.

```csharp
public static class StorageHelper
{
    public static IUserStorage GetUserStorage(IThinClientContext context)
    {
        if (string.IsNullOrEmpty(context.User?.Id))
            throw new NotAuthenticated();

        return context.ApplicationStorage;
    }
}
```

If a feature is device-only, use a separate helper returning `context.DeviceStorage`.

## Usage

```csharp
var storage = StorageHelper.GetUserStorage(context);
var store = await storage.Get<MyStore>(MyStore.Key) ?? new MyStore { Id = MyStore.Key };

store.Name = request.Name;
await storage.Store(store);
```

## Registration

Every type you persist must be registered in `OnInit` so the framework can serialize and deserialize it correctly.

```csharp
RegisterEntity<ContactStore>();
RegisterEntity<SettingsData>();
```

## Notes

- Singleton-style records should use a fixed key.
- Reuse helper methods rather than spreading storage selection logic across actions.
- Keep storage entities simple and stable.
