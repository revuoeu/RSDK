# Agent Control — Creating GUI controls (Revuo)

Short reference for adding UI controls to your app — copyable steps, examples, and checklist.

## 🔎 Overview
- Controls inherit framework base classes: `BasePayloadControlThinClient<T, TApp>`.
- UI actions should load data in the *Client app* (not in GUI components).
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
   - Create `.razor` + optional code‑behind inheriting `BasePayloadControl*`.
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
@inherits BasePayloadControlFatClient<ShowFooRequest, Foo.Client.FooApp>

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
- Load data in Client app actions — GUI only displays already-loaded payloads.
- Keep `HasActions => true` when you want automatic action buttons.
- Use `AssemblyInformationalVersion`/`InformationalVersion` for build metadata — not for binding.
- Unit test both action results and control rendering.

---

## ✅ Quick checklist before merge
- [ ] Payload type defined in Model
- [ ] Client action returns populated payload
- [ ] `.razor` control created and inherits `BasePayloadControl*`
- [ ] `HasActions` set if needed
- [ ] `services.AddControl<,>()` registration added
- [ ] Translation keys added and designer regenerated
- [ ] Unit test added

---

If you want, I can create a ready template control (razor + code‑behind + IoC + test + resx) in a target app — tell me which app. 
