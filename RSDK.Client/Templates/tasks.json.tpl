{
  "version": "2.0.0",
  "tasks": [
    {
        "type": "shell",
        "label": "Run Workflow",
        "command": "C:\\Work\\Sandbox\\Chat\\Src\\Revuo.Chat.Client\\bin\\Debug\\net10.0-windows10.0.19041.0\\win-x64\\Revuo.Chat.Client.exe",
        "args": [
            "workflow",
            "${workspaceFolder}\\InstallRSDKWorkflow.json"
        ],
        "dependsOn": ["build RSDK.Client"]
    },
    {
        "type": "process",
        "command": "dotnet",
        "label": "build RSDK.Client",
        "args": [
            "build",
            "${workspaceFolder}/{{ProjectName}}.csproj",
            "-c",
            "Debug"
        ],
        "group": "build",
        "presentation": {
            "echo": true,
            "reveal": "always",
            "focus": false,
            "panel": "shared",
            "showReuseMessage": true,
            "clear": false
        },
        "problemMatcher": "$msCompile"
    }
  ]
}

