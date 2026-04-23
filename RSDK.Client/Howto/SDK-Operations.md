# SDK Operations

This file consolidates the generic console and operational workflows that were missing from the SDK topic docs.

## Console location

Most SDK console workflows start in:

```powershell
Set-Location "C:\Work\Sandbox\Chat\Src\Revuo.Chat.Console"
```

## Create a new application

```powershell
dotnet run -- sdk create
```

The generated project is typically placed under:

```text
%USERPROFILE%\Documents\Revuo.SDK.Client\CSharpProjects\
```

After generation:

```powershell
dotnet restore
dotnet build
dotnet run
```

## Load an application from a folder

For actions that need a payload, first generate a template:

```powershell
dotnet run -- show arguments --application SDK --action Load > load_payload.json
```

Then edit the payload:

```json
{
  "path": "C:\\Users\\YourUser\\Documents\\MyApp",
  "name": "My Application",
  "culture": "en-US"
}
```

Run it:

```powershell
dotnet run -- run --application SDK --action Load --parameter-file load_payload.json
```

## Generate payload templates

```powershell
dotnet run -- show arguments --application SDK --action Load
dotnet run -- show arguments --application Chat --action LoadConversation
```

Use templates as the source of truth for payload shape before calling an action.

## Run an action with a payload

```powershell
dotnet run -- run --application SDK --action Load --parameter-file payload.json
```

For parameterless actions:

```powershell
dotnet run -- run --application SDK --action ListProjects
```

## Registration and approval

Use the registry flow when app installation must be approved and version-pinned.

High-level flow:

1. developer loads a build from GitHub
2. developer submits a registration request
3. root admin approves or rejects it
4. end users install the approved app from the registry

Key rule: the resolved tag should stay pinned so users receive the exact approved build.

## Self-hosted app server

Some apps can run outside the shared server process.

Typical flow:

```powershell
dotnet build -c Release
dotnet run -- install --path "path\to\YourApp.Server\bin\Release\net10.0"
dotnet run -- serve --application YourApp
```

Use this when the app owns its own feed or background processing and should run on a private machine or server.

## Open public links

The deep-link flow is:

1. a URL carries a public link id
2. the client resolves that id through the server
3. the response is shown in the UI

If an in-app action should open the result immediately, prefer a request/response flow that returns the payload to the caller instead of fire-and-forget messaging.

## Practical notes

- use Windows paths in payload JSON
- validate JSON before running it
- list applications when an app key is uncertain
- use `--help` on console commands when needed
