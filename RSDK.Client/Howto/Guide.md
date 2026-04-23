# Feature Guide

Use this workflow when adding a new feature to an RSDK app.

## Global rule

- do not add new NuGet packages
- stay within the packages and project references already approved for the repo
- if a capability is missing, prefer existing framework features or shared code already present in the solution

## 1. Define the model

- add payloads and entities in `Models`
- decide which types are transport-only and which are persisted
- use `BasePayload<T>`, `BasePayloadWithErrors<T>`, and `BasePayloadEntity` where appropriate

## 2. Add the action

- register the action in `OnInit`
- keep action logic stateless
- load data in the action, not in the control
- add auth checks before user-scoped storage access

## 3. Add the control

- create `.razor` + `.razor.cs`
- inherit from `BasePayloadControlThinClient<TPayload, TApp>`
- copy payload values into local fields for binding
- use Bootstrap classes and simple markup
- decide whether the automatic action bar is useful

## 4. Add translations

- add action labels and UI text to `I18N.set`
- keep key names stable and predictable
- update every supported language you maintain

## 5. Wire persistence

- use a storage helper
- register persisted types with `RegisterEntity<T>()`
- use fixed IDs for singleton records

## 6. Test it

- the SDK scaffolds a `{ProjectName}.Test` project automatically — open it and start writing tests there
- add action-level unit tests for business logic (storage reads/writes, auth checks, payload mapping)
- add plain model tests for payloads, stores, and helpers — no DI needed, fast to run
- add control render tests with bUnit when visual behavior matters
- use FakeItEasy for faking dependencies
- instantiate a control class directly when you only need to test code-behind behavior

See `Testing.md` for concrete examples.
