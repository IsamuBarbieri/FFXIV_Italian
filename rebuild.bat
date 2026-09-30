@echo off
echo ==================================================
echo  FFXIV Italiano - Rebuild Mod
echo ==================================================
cd /d "%~dp0"
dotnet run --configuration Release --project src/FFXIVItalian.Patcher
echo.
pause

