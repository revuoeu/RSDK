{
  "version": "0.2.0",
  "configurations": [
    {
        "name": "Debug in Revuo Client (EXE)",
        "type": "coreclr",
        "request": "attach",
        "processName": "Revuo.Chat.Client.exe",
        "preLaunchTask": "Run Workflow"
    },
    {
        "name": "Debug in Revuo Client (revuo://)",
        "type": "coreclr",
        "request": "attach",
        "processName": "Revuo.Chat.Client.exe",
        "preLaunchTask": "Run Workflow 2"
    }
  ]
}
