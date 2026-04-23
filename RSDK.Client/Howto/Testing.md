# Testing

Use a mix of action tests, render tests, and direct control tests.

## Recommended stack

- **bUnit** for Razor component rendering
- **FakeItEasy** for faking dependencies and collaborators
- direct **control instantiation** for code-behind logic that does not need a renderer

## What to test

### Action logic

Test:

- storage reads and writes
- auth checks
- returned payload shape
- business errors in `BasePayloadWithErrors<T>`

Fake external collaborators with FakeItEasy instead of hitting real services.

## Rendering tests with bUnit

Use bUnit when you need to verify markup, bindings, conditional rendering, or user interaction.

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

If the project uses plain bUnit helpers instead of `RenderRevuoComponent`, follow the local test pattern already used in that repo.

## Faking dependencies with FakeItEasy

Use FakeItEasy to isolate the unit under test.

```csharp
var service = A.Fake<IMyService>();
A.CallTo(() => service.Load()).Returns(Task.FromResult("value"));
```

Register the fake in the test DI container or pass it into the class under test, depending on the existing project pattern.

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

Use direct instantiation only when the behavior does not require Blazor lifecycle rendering or DOM assertions. This works best for helper methods, validation routines, or small state transitions on the `.razor.cs` partial class. If markup, event wiring, or component lifecycle matters, use bUnit instead.

## Suggested coverage

- action tests for application logic
- bUnit tests for important screens
- direct unit tests for control helper methods
- one test per bug fix when a UI issue is easy to reproduce in code
