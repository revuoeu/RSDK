# Agent Control — Creating GUI controls (Revuo)

Short reference for adding UI controls to your app — copyable steps, examples, and checklist.

## 🔎 Overview
- Controls inherit framework base classes: `BasePayloadControlThinClient<T, TApp>`.
- Use Bootstrap for layout and styling in UI controls (prefer `row`, `col-*`, `form-control`, `btn`, etc.).
- UI actions should load data in the *Client app* (not in GUI components).
- For GUI projects that contain `.razor` controls, set the project SDK to `Microsoft.NET.Sdk.Razor` (i.e. `<Project Sdk="Microsoft.NET.Sdk.Razor">`).
- Register every control in application Init (`this.AddControl<YourControl>()`).
- Add translations in the Model `.resx` and rebuild to regenerate designer classes.

---

## ✅ When to use
- New list/detail screens, forms, or sub‑components (CRUD). 
- Navigation between related views using type-safe payloads.

---

## 🛠 Minimal workflow (step‑by‑step)
1. Model: define payload types (use `BasePayload<T>` / `BasePayloadEntity`).
2. Client app:
   - Add a UI action that loads data and returns a payload (use `AddAction(...)`).
3. GUI component:
   - Create `.razor` + code‑behind (`.razor.cs`) and prefer splitting markup and C# into the two files; e.g. `NewProjectControl.razor` + `NewProjectControl.razor.cs`, `SdkSettingsControl.razor` + `SdkSettingsControl.razor.cs`. Ensure the code‑behind declares the base class `BasePayloadControlThinClient<...>` or add `@namespace RSDK.Client` in the `.razor` so the app type resolves.
   - Set `HasActions => true` to enable action discovery.
   - Use `RunAction<TResponse>("ActionName", request)` and `ParentFrame.Show(...)` for navigation.
4. Application Init: register the control with `this.AddControl<YourControl>()` and add translations.
5. Test: render the control with `RenderRevuoComponent<...>(...)`.

---

## ✏️ Example — Client action (loads data)
```csharp
// Client app (OnInit)
AddAction(ShowFooManagement);

private async Task<ShowFooRequest> ShowFooManagement(IThinClientContext ctx) {
  var items = await LoadFoos();
  return new ShowFooRequest(new FooListData { Items = items });
}
```

## ✨ Example — GUI control (.razor / code‑behind)
```razor
@using Microsoft.AspNetCore.Components.Web

@namespace RSDK.Client

@inherits Revuo.Chat.Client.Base.Abstractions.BasePayloadControlThinClient<SdkSettings, SDKApp>

<input class="form-control" @bind="Payload.SomeField" @bind:event="oninput" />

@code {
  public override bool HasActions => true;

  protected override async Task OnInitializedAsync() {
    if (Payload?.D != null) Items = Payload.D.Items;
    await base.OnInitializedAsync();
  }

  private async Task OpenCreate() {
    var req = await RunAction<ShowCreateFooRequest>("ShowCreateFoo");
    await ParentFrame.Show(req); // type-safe navigation
  }
}
```

---

## 🌐 Translations
- Add keys in `Translation{App}.resx` (e.g. `Foo.Client.FooApp.ShowFooManagement`).
- Rebuild to regenerate `Translations{App}.Designer.cs`.

---

## 🧩 Navigation & patterns
- Use `ParentFrame.Show(payload)` — preferred for navigation. 
- Use `DialogService.OpenApplication()` for app-level dialogs.
- Prefer separate sub‑components for CRUD (no boolean modal flags).

---

## 🧪 Tests (example)
```csharp
var cut = RenderRevuoComponent<FooManagementControl, Foo.Client.FooApp, Foo.Model.FooTranslations, ShowFooRequest>(request);
Assert.Contains("FooManagement", cut.Markup);
```

---

## ⚠️ Gotchas & best practices
- For projects that contain `.razor` components, ensure the project SDK is `Microsoft.NET.Sdk.Razor` — otherwise Razor controls will not compile.
- In `.razor` controls include the full namespace for the application type in the `@inherits` generic (or add `@namespace`) to avoid missing-type or ambiguous-type errors.
- Load data in Client app actions — GUI only displays already-loaded payloads.
- Keep `HasActions => true` when you want automatic action buttons.
- Use `AssemblyInformationalVersion`/`InformationalVersion` for build metadata — not for binding.
- Unit test both action results and control rendering.

---

## ✅ Quick checklist before merge
- [ ] Project file uses `<Project Sdk="Microsoft.NET.Sdk.Razor">` (for `.razor` UI controls)
- [ ] Payload type defined in Model
- [ ] Client action returns populated payload
- [ ] `.razor` control created and inherits `BasePayloadControl*` (ensure `@inherits` includes the app namespace or add `@namespace`)
- [ ] `HasActions` set if needed
- [ ] `services.AddControl<,>()` registration added
- [ ] Translation keys added and designer regenerated
- [ ] Unit test added

---

If you want, I can create a ready template control (razor + code‑behind + IoC + test + resx) in a target app — tell me which app. 
