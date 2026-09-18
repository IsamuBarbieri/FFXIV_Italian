@echo off
echo ==================================================
echo  FFXIV Italiano - Rebuild Mod
echo ==================================================
cd /d "%~dp0"
dotnet run --project src/FFXIVItalian.Patcher
echo.
pause

