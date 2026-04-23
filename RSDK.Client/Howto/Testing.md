# Testing

Use a mix of action tests, plain model tests, bUnit render tests, and direct control tests.

## Recommended stack

- **xUnit** for test structure (`[Fact]`, not NUnit `[Test]`)
- **bUnit** for Razor component rendering
- **FakeItEasy** for faking dependencies and collaborators
- direct **control instantiation** for code-behind logic that does not need a renderer

## Project setup

Create a separate test project that references the app project:

```xml
<!-- YourApp.Test/YourApp.Test.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="bunit" Version="1.37.7" />
    <PackageReference Include="FakeItEasy" Version="8.3.0" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\YourApp\YourApp.csproj" />
  </ItemGroup>
</Project>
```

If the repository already has an established test project or slightly different versions, follow the local pattern instead of forcing this exact template.

## What to test

### Action logic

Test:

- storage reads and writes
- auth checks
- returned payload shape
- business errors in `BasePayloadWithErrors<T>`

Fake external collaborators with FakeItEasy instead of hitting real services.

### Plain model and mapping tests

Test plain C# classes with no Blazor or DI involvement: payloads, stores, mappers, serialization, and small helpers.

```csharp
public class MyModelTests
{
    [Fact]
    public void MyEntity_Property_SurvivesJsonRoundTrip()
    {
        var original = new MyEntity { Value = "hello" };
        var json = JsonSerializer.Serialize(original);
        var loaded = JsonSerializer.Deserialize<MyEntity>(json)!;

        Assert.Equal(original.Value, loaded.Value);
    }

    [Fact]
    public void Payload_MapsFrom_StoreCorrectly()
    {
        var store = new MyStore { Name = "test" };
        var payload = new MyPayload { Name = store.Name };

        Assert.Equal("test", payload.Name);
    }
}
```

These are the fastest tests and should cover as much non-UI logic as possible.

## Rendering tests with bUnit

Use bUnit when you need to verify markup, bindings, conditional rendering, lifecycle-driven state, or user interaction.

If the project already has a helper such as `RenderRevuoComponent`, follow that local pattern. Otherwise use plain `RenderComponent(...)` with explicit parameters and DI setup.

### Minimal render example

```csharp
using Bunit;
using FakeItEasy;
using Xunit;

public class SettingsControlTests : TestContext
{
    [Fact]
    public void Renders_name_from_payload()
    {
        var request = new SettingsData { Name = "SDK" };

        var cut = RenderRevuoComponent<SettingsControl, SDKApp, SDKTranslations, SettingsData>(request);

        Assert.Contains("SDK", cut.Markup);
    }
}
```

### Full bUnit test class template

```csharp
using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Revuo.Chat.Abstraction;
using Revuo.Chat.Abstraction.Client;
using Revuo.Chat.Abstraction.Models;
using Revuo.Chat.Base.I18N;

namespace YourApp.Test;

public class MyControlViewTests : TestContext
{
    private readonly MyApp _app = new();
    private readonly IThinClientContext _ctx;

    public MyControlViewTests()
    {
        Services.AddSingleton(A.Fake<ICultureManager>());

        var dialogService = A.Fake<IDialogService>();
        A.CallTo(() => dialogService.Handle(A<Func<Task<IPayload>>>._))
            .Returns(Task.FromResult<IPayload>(new MyInitPayload()));
        Services.AddSingleton(dialogService);

        var storage = A.Fake<IUserStorage>();
        A.CallTo(() => storage.Get<MyStore>(A<string>._))
            .Returns(Task.FromResult<MyStore?>(null));

        var user = new UserReference { Id = "test-user" };

        _ctx = A.Fake<IThinClientContext>();
        A.CallTo(() => _ctx.ApplicationStorage).Returns(storage);
        A.CallTo(() => _ctx.User).Returns(user);
    }

    private IRenderedComponent<MyControlView> Render(MyControlPayload payload) =>
        RenderComponent<MyControlView>(p => p
            .Add(c => c.Payload, payload)
            .Add(c => c.Application, _app)
            .Add(c => c.Ctx, _ctx));

    [Fact]
    public void EmptyPayload_ShowsExpectedState()
    {
        var cut = Render(new MyControlPayload());

        Assert.Contains("expected-css-class", cut.Find(".my-element").ClassName);
    }
}
```

## Required DI services

Controls that inherit from the Revuo base control stack use `[Inject]` services. All required services must be registered or bUnit will fail with an error like:

```text
System.InvalidOperationException: Cannot provide a value for property 'DialogService'
  on type '...'. There is no registered service of type 'IDialogService'.
```

Common required services:

| Service | Namespace | Notes |
|---------|-----------|-------|
| `ICultureManager` | `Revuo.Chat.Base.I18N` | Usually always required |
| `IDialogService` | `Revuo.Chat.Abstraction.Client` | Needs special setup; see below |

Your own control may inject more services. When a test fails to render, the exception usually tells you exactly which type is missing.

## The `IDialogService.Handle` trap

`RunAction` on the base control ultimately calls `DialogService.Handle(...)`. A plain `A.Fake<IDialogService>()` often returns a proxy object from `Handle`, and the later cast to the expected payload type fails with `InvalidCastException`.

**Mock known payload from calling RunAction on ui:**

```csharp
A.CallTo(() => dialogService.Handle(A<Func<Task<IPayload>>>._))
    .Returns(Task.FromResult<IPayload>(new MyInitPayload()));
```

Use this when the control calls `RunAction` during first render and the test only needs predictable initialization data.

## `IThinClientContext` setup

Controls often reach storage through `ctx.ApplicationStorage` and assume a valid user identity in `ctx.User.Id`.

```csharp
var storage = A.Fake<IUserStorage>();
A.CallTo(() => storage.Get<MyStore>(A<string>._))
    .Returns(Task.FromResult<MyStore?>(null));

var user = new UserReference { Id = "test-user" };

var ctx = A.Fake<IThinClientContext>();
A.CallTo(() => ctx.ApplicationStorage).Returns(storage);
A.CallTo(() => ctx.User).Returns(user);
```

Return `null` by default for storage reads unless the test needs a real stored object. The control should handle missing storage entries gracefully.

### Why `new UserReference` instead of `A.Fake<UserReference>()`?

`UserReference.Id` is a non-virtual property. FakeItEasy cannot intercept non-virtual members on concrete classes, so faking it leads to `FakeConfigurationException`. Construct concrete Revuo model classes directly when they expose non-virtual properties.

## Component parameters

`BasePayloadControlGeneric`-style components expose these common Blazor parameters:

| Parameter | Type |
|-----------|------|
| `Payload` | `TPayload` |
| `Application` | `TApp` |
| `Ctx` | `IThinClientContext` |

Use strongly typed lambdas when rendering to avoid silent parameter-name mistakes:

```csharp
RenderComponent<MyControlView>(p => p
    .Add(c => c.Payload, payload)
    .Add(c => c.Application, app)
    .Add(c => c.Ctx, ctx));
```

## `OnAfterRenderAsync` and default values

Many controls set defaults or call `RunAction(...)` on first render. That can overwrite incoming payload values and change what the test sees after render.

Typical pattern:

```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (!firstRender || Payload is null) return;

    Payload.Name = "Default name";
    var settings = await RunAction<SettingsPayload>("GetSettings", null);
    _hasApiKey = !string.IsNullOrEmpty(settings?.ApiKey);

    if (string.IsNullOrEmpty(Payload.BodyHtml))
        Payload.BodyHtml = "Default body";
}
```

Implications for tests:

- pre-populate fields that should not be overwritten during render
- remember that first-render `RunAction(...)` calls usually require the `IDialogService.Handle` stub
- assert against post-render state, not only constructor-time input

## Asserting markup

```csharp
Assert.Contains("alert-warning", cut.Find(".alert").ClassName);

var badge = cut.Find(".badge.text-bg-success");

Assert.Contains("expected text", cut.Markup);

var input = cut.Find("#myField");
Assert.Equal("expected value", input.GetAttribute("value"));

var textarea = cut.Find("#myTextarea");
Assert.Equal("expected text", textarea.GetAttribute("value"));
```

For `@bind`, Blazor stores the current value in the HTML `value` attribute. In bUnit, `TextContent` is often not the right assertion for `<input>` and `<textarea>` elements.

## Instantiating a control directly

If you want to test code-behind methods or state transitions without rendering, instantiate the control class directly.

This is useful for:

- helper methods on the `.razor.cs` partial class
- validation logic
- local state mutations that do not depend on the render tree

```csharp
var control = new SettingsControl();

var result = control.NormalizeName("  SDK  ");

Assert.Equal("SDK", result);
```

Use direct instantiation only when the behavior does not require Blazor lifecycle rendering, injected services, or DOM assertions.

## Troubleshooting

| Error | Cause | Fix |
|-------|-------|-----|
| `Cannot provide a value for property 'X'` | Missing DI registration | Register the missing service in `Services` |
| `InvalidCastException` from proxy to payload type | `IDialogService.Handle` returned a fake proxy | Stub `Handle` to return a real payload or call through |
| `Application action not found for X` | App initialization did not register actions | Return a known payload instead of executing the real action |
| `FakeConfigurationException` on concrete members | Faking a non-virtual property on a concrete class | Construct the class with `new` |
| Assertion fails on `<textarea>` value | `@bind` writes to the `value` attribute, not `TextContent` | Assert using `GetAttribute("value")` |
| Payload value changes after render | `OnAfterRenderAsync` overwrote defaults | Pre-populate the fields that prevent the overwrite |

## Suggested coverage

- action tests for application logic
- plain unit tests for payloads, stores, and mapping helpers
- bUnit tests for important screens and stateful controls
- direct unit tests for control helper methods
- one focused regression test for each reproducible bug fix
