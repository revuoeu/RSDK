# Storage Access

This document describes the recommended pattern for accessing storage in a Revuo app.

## Why use a helper

Many actions need to read or write persistent data via `context.ApplicationStorage`.
Because storage is user-scoped, each action should first verify that the caller is authenticated.

The app should **throw `NotAuthenticated`** if `context.User.Id` is null or empty.

## Recommended helper

Add a small helper method that centralizes this check and returns the storage instance.

```csharp
public static class StorageHelper
{
    public static IUserStorage GetStorage(IThinClientContext context)
    {
        if (string.IsNullOrEmpty(context.User?.Id))
            throw new NotAuthenticated();

        return context.ApplicationStorage;
    }
}
```

## Usage

Use the helper whenever reading or writing storage in an action:

```csharp
var storage = StorageHelper.GetStorage(context);
var store = await storage.Get<MyStore>(MyStore.Id);
```

This ensures all storage access is guarded by the same authentication check, and keeps action logic clean.
