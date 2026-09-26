{
  "version": "2.0.0",
  "tasks": [
    {
        "type": "shell",
        "label": "Run Workflow",
        "command": {{RevuoChatClientExePathJson}},
        "args": [
            "workflow",
            "${workspaceFolder}\\InstallWorkflow.json"
        ],
        "dependsOn": ["build RSDK.Client"]
    },
    {
        "label": "Run Workflow 2",
        "type": "shell",
        "command": "Start-Process 'revuo://workflow?file=${workspaceFolder}/InstallWorkflow.json'",
        "dependsOn": ["build RSDK.Client"]
    },
    {
        "type": "process",
        "command": "dotnet",
        "label": "build RSDK.Client",
        "args": [
            "build",
            "${workspaceFolder}/{{ProjectName}}/{{ProjectName}}.csproj",
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

