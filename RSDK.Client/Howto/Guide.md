# Translations — application actions (how‑to) 🔧

Add translation entries for actions that your application registers with `this.AddAction(...)`.
This guide uses `Foo.App` as a neutral example namespace — replace `Foo.App` with your app identifier where appropriate.

## Key format

- Use `Foo.App.<ActionName>` as the translation key.
- `<ActionName>` must match the method name passed to `AddAction` (exact spelling and casing).
> Tip: keep the translation key namespace (`Foo.App`) identical to the value you pass to `context.RunAction(...)` so it's easy to find and maintain keys.

## Example — add to `I18N.cs`

Add the keys under each language's `Entries` section. Example snippets (replace `Foo.App` with your app id):

`en-US`:

```csharp
["Foo.App.NewProject"] = "New project",
["Foo.App.CreateNewProject"] = "Create new project",
["Foo.App.CreateNewProject_CreateFolder"] = "Creating project folder",
["Foo.App.GetSdkSettings"] = "Load settings",
["Foo.App.SaveSdkSettings"] = "Save settings",
```

`pl-PL`:

```csharp
["Foo.App.NewProject"] = "Nowy projekt",
["Foo.App.CreateNewProject"] = "Utwórz nowy projekt",
["Foo.App.CreateNewProject_CreateFolder"] = "Tworzenie folderu projektu",
["Foo.App.GetSdkSettings"] = "Wczytaj ustawienia",
["Foo.App.SaveSdkSettings"] = "Zapisz ustawienia",
```

> Note: the application typically constructs a `StaticTranslator(I18N.set)` so entries in `I18N.set` are available at runtime. Rebuild the project after editing `I18N.cs` to pick up new translations.
