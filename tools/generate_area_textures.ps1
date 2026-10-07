param(
    [string]$ConfigPath = (Join-Path $PSScriptRoot 'AreaTextureTool/area_texture_labels.json'),
    [string]$FullScreenReferencePsdPath = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\Duty Complete font.psd')),
    [switch]$ReloadStyle
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'AreaTextureTool/AreaTextureTool.csproj'
$configPath = [IO.Path]::GetFullPath($ConfigPath)
$previewRoot = Join-Path $PSScriptRoot 'fullscreen_texture_previews'
$stylesRoot = Join-Path $PSScriptRoot 'AreaTextureTool/Styles'
$jsxPath = Join-Path $PSScriptRoot 'AreaTextureTool/Styles/apply_zone_style.jsx'
$referencePsdPath = [IO.Path]::GetFullPath($FullScreenReferencePsdPath)
$batchConfigPath = Join-Path $previewRoot '.area-texture-batch.json'

New-Item -ItemType Directory -Path $previewRoot -Force | Out-Null
$config = Get-Content $configPath -Raw | ConvertFrom-Json
$entries = @($config.entries)
if (@($entries | Where-Object layout -eq 'full').Count -gt 0 -and -not (Test-Path -LiteralPath $referencePsdPath)) {
    throw "PSD di riferimento full screen non trovato: $referencePsdPath"
}
foreach ($entry in $entries) {
    if ($entry.layout -notin @('region', 'zone', 'full') -or [string]::IsNullOrWhiteSpace($entry.text)) {
        throw "Testo o layout non valido per $($entry.id)."
    }
}
$batchTotal = 0
foreach ($layout in @('region', 'zone', 'full')) {
    $layoutEntries = @($entries | Where-Object layout -eq $layout)
    $batchTotal += [Math]::Ceiling($layoutEntries.Count / 4)
}
$batchNumber = 0
$photoshopCallNumber = 0
$photoshop = New-Object -ComObject Photoshop.Application
foreach ($layout in @('region', 'zone', 'full')) {
    $layoutEntries = @($entries | Where-Object layout -eq $layout)
    $stylePath = Join-Path $stylesRoot $(switch ($layout) { 'region' { 'Region Style.ASL' } 'zone' { 'Zone Style.ASL' } 'full' { 'FF Full Screen.ASL' } })
    $styleName = switch ($layout) { 'region' { 'Region Scl' } 'zone' { 'Zone Style' } 'full' { 'FF Full Screen' } }
    for ($offset = 0; $offset -lt $layoutEntries.Count; $offset += 4) {
        $batchEntries = @($layoutEntries | Select-Object -Skip $offset -First 4)
        $tasks = foreach ($entry in $batchEntries) {
            foreach ($suffix in @('', '_hr1')) {
                if ($entry.layout -notin @('region', 'zone', 'full') -or [string]::IsNullOrWhiteSpace($entry.text)) {
                    throw "Testo o layout non valido per $($entry.id)."
                }
                [pscustomobject]@{
                    text = $entry.text
                    layout = $entry.layout
                    height = $(if ($entry.height -gt 0) { [int]$entry.height } elseif ($entry.layout -eq 'full') { 360 } elseif ($entry.layout -eq 'region') { 64 } else { 128 })
                    scale = $(if ($suffix) { 6 } else { 3 })
                    maskPath = Join-Path $previewRoot "$($entry.id)$suffix.mask.png"
                    styledPath = Join-Path $previewRoot "$($entry.id)$suffix.styled.png"
                }
            }
        }
        $styleJson = ConvertTo-Json -InputObject $stylePath -Compress
        $referencePsdJson = ConvertTo-Json -InputObject $referencePsdPath -Compress
        $styleNameJson = ConvertTo-Json -InputObject $styleName -Compress
        $batchNumber++
        Write-Output "Blocco $batchNumber / $batchTotal, $layout ($($batchEntries[0].id)–$($batchEntries[-1].id))"
        foreach ($task in $tasks) {
            Remove-Item -LiteralPath $task.maskPath, $task.styledPath -Force -ErrorAction SilentlyContinue
            $taskJson = '[' + (ConvertTo-Json -InputObject $task -Depth 3 -Compress) + ']'
            $reloadStyleValue = if ($ReloadStyle -and $photoshopCallNumber -eq 0) { 'true' } else { 'false' }
            $jsx = (Get-Content $jsxPath -Raw).Replace('__TASKS_JSON__', $taskJson).Replace('__STYLE_PATH__', $styleJson).Replace('__REFERENCE_PSD_PATH__', $referencePsdJson).Replace('__STYLE_NAME__', $styleNameJson).Replace('__RELOAD_STYLE__', $reloadStyleValue)
            $photoshopCallNumber++
            Write-Output "Rendering $($task.text) ($($task.scale)x)"
            $photoshopResult = $photoshop.DoJavaScript($jsx)
            if ($photoshopResult.StartsWith('ERROR:')) {
                throw $photoshopResult
            }
            Write-Output $photoshopResult
        }

        @{ entries = $batchEntries } | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $batchConfigPath -Encoding utf8
        dotnet run --project $project -- pack --config $batchConfigPath | Out-Null
        if ($LASTEXITCODE -ne 0) {
            throw "La creazione delle texture TEX è terminata con codice $LASTEXITCODE nel blocco $batchNumber."
        }

        foreach ($task in $tasks) {
            Remove-Item -LiteralPath $task.maskPath, $task.styledPath -Force -ErrorAction SilentlyContinue
        }
    }
}
Remove-Item -LiteralPath $batchConfigPath -Force -ErrorAction SilentlyContinue
Write-Output "Generate $($entries.Count) texture con varianti standard e HR1."
