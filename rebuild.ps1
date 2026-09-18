# FFXIV Italiano - One-Click Rebuild Script
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host " FFXIV Italiano - Rebuild Mod" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

dotnet run --project src/FFXIVItalian.Patcher
