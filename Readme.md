# RSDK - Revuo Software Development Kit

This project is an SDK for developing Revuo applications. It provides the necessary tools and client libraries to build, deploy, and manage Revuo-based solutions.

## Usage

### Build locally

- Restore & build the client library:

  `dotnet restore ./RSDK.Client/RSDK.Client.csproj`
  `dotnet build ./RSDK.Client/RSDK.Client.csproj --configuration Release`

- Built DLL location:

  `RSDK.Client/bin/Release/net10.0/RSDK.Client.dll`

### Run examples

- Example installer client calls (local tools):

  `# ..\Chat\Bin\revuo-chat.exe run -ic -a Installer.Client.InstallerApp -act TemporaryInstallFromDll -pf C:\Work\Sandbox\RSDK\InstallationRequest.json`
  `..\Chat\Bin\revuo-chat.exe workflow -f C:\Work\Sandbox\RSDK\workflow.json -o`

### CI / Releases (GitHub Actions) 🔧

This repository includes a workflow at `.github/workflows/build-and-release.yml` that:
- Builds `RSDK.Client` in Release configuration
- Locates `RSDK.Client.dll` and uploads it to the corresponding GitHub Release

Workflow triggers:
- Push a tag that matches `v*` (example: `v1.0.0`) — workflow creates a Release and attaches the DLL
- `release.published` — attach DLL to an already-created Release
- Manual run from Actions tab (`workflow_dispatch`)

Create a release by tagging and pushing:

```
git tag v1.0.0
git push origin --tags
```

After the workflow completes the compiled `RSDK.Client.dll` will appear under the Release's **Assets**.

### Customization & notes 💡

- To also attach PDB / XML docs or a ZIP, I can update the workflow to include those files.
- Workflow file: `.github/workflows/build-and-release.yml` — edit if you need different triggers or artifact names.

---

If you want, I can add PDB/XML attachments or change the workflow to produce a zip artifact — tell me which option you prefer.
