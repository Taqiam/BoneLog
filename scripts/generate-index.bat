@echo off
setlocal EnableExtensions

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%.."

pushd "%REPO_ROOT%"
dotnet run "%SCRIPT_DIR%GenerateIndex.cs" -- ^
  "%REPO_ROOT%\src\BoneLog.Blazor\wwwroot\data\posts" ^
  "%REPO_ROOT%\src\BoneLog.Blazor\wwwroot\data\index.json"
set "EXIT_CODE=%ERRORLEVEL%"
popd

if not "%EXIT_CODE%"=="0" pause
exit /b %EXIT_CODE%
