param(
    [string[]]$Ids = @()
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'ClanTextureGenerator/ClanTextureGenerator.csproj'
dotnet run --project $project -- @Ids
if ($LASTEXITCODE -ne 0) {
    throw "Il generatore delle texture dei clan è terminato con codice $LASTEXITCODE."
}
