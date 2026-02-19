{
  "version": "2.0.0",
  "tasks": [
    {
      "type": "process",
      "label": "build {{ProjectName}}",
      "command": "dotnet",
      "args": [ "build", "${workspaceFolder}/{{ProjectName}}.csproj", "-c", "Debug" ],
      "group": "build",
      "presentation": { "echo": true, "reveal": "always", "focus": false, "panel": "shared", "showReuseMessage": true, "clear": false },
      "problemMatcher": "$msCompile"
    },
    {
      "type": "process",
      "label": "test {{ProjectName}}",
      "command": "dotnet",
      "args": [ "test", "${workspaceFolder}/{{ProjectName}}.csproj", "-c", "Debug" ],
      "group": "test",
      "presentation": { "echo": true, "reveal": "always", "focus": false, "panel": "shared", "showReuseMessage": true, "clear": false }
    }
  ]
}
