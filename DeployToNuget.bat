@echo off
nuget.exe update -self
ECHO Y | DEL *.nupkg

set /p NuGetApiKey= Please enter the project's NuGet API Key: 
nuget.exe setApiKey %NuGetApiKey%
nuget.exe pack BattleNET\BattleNET.csproj
nuget.exe push *.nupkg