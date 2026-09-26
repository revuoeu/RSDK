{
  "Name": "Install application from dll",
  "Description": "Install application, check if it is visible on list and run some action.",
  "Steps": [
    { "Order": 1, "ApplicationName": "Installer.Client.InstallerApp", "ActionName": "InstallFromDll", "ParameterFilePath": {{InstallationRequestPathJson}}, "ContinueOnError": true },
    { "Order": 2, "ApplicationName": "Installer.Client.InstallerApp", "ActionName": "ListApplications", "ParameterFilePath": null, "ContinueOnError": true },
    { "Order": 3, "ApplicationName": "RSDK.Client.SDKApp", "ActionName": "SelectNewProjectType", "ParameterFilePath": null, "ContinueOnError": true }
  ]
}
