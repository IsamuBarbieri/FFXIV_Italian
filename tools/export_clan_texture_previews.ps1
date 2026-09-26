param(
    [string]$InputDirectory = (Join-Path $PSScriptRoot '..\data\assets\ui\icon\126000\en'),
    [string]$OutputDirectory = (Join-Path $PSScriptRoot 'clan_texture_previews'),
    [string[]]$Ids = @()
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$inputPath = (Resolve-Path -LiteralPath $InputDirectory).Path
$outputPath = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

foreach ($file in Get-ChildItem -LiteralPath $inputPath -Filter '*.tex' -File) {
    if ($Ids.Count -gt 0 -and $file.BaseName -notin @($Ids + ($Ids | ForEach-Object { "${_}_hr1" }))) { continue }
    $bytes = [IO.File]::ReadAllBytes($file.FullName)
    if ($bytes.Length -lt 80 -or [BitConverter]::ToUInt32($bytes, 4) -ne 0x1450) {
        Write-Warning "Formato non supportato, ignorato: $($file.Name)"
        continue
    }

    $width = [BitConverter]::ToUInt16($bytes, 8)
    $height = [BitConverter]::ToUInt16($bytes, 10)
    $pixelOffset = [BitConverter]::ToUInt32($bytes, 28)
    $pixelBytes = [int]$width * [int]$height * 4
    if ($pixelOffset -lt 80 -or $bytes.Length -lt $pixelOffset + $pixelBytes) {
        Write-Warning "Pixel incompleti, ignorato: $($file.Name)"
        continue
    }

    $bitmap = [Drawing.Bitmap]::new($width, $height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $rectangle = [Drawing.Rectangle]::new(0, 0, $width, $height)
    $data = $bitmap.LockBits($rectangle, [Drawing.Imaging.ImageLockMode]::WriteOnly, $bitmap.PixelFormat)
    [Runtime.InteropServices.Marshal]::Copy($bytes, [int]$pixelOffset, $data.Scan0, $pixelBytes)
    $bitmap.UnlockBits($data)
    $destination = Join-Path $outputPath ($file.BaseName + '.png')
    $bitmap.Save($destination, [Drawing.Imaging.ImageFormat]::Png)
    $preview = [Drawing.Bitmap]::new($width, $height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [Drawing.Graphics]::FromImage($preview)
    $graphics.Clear([Drawing.Color]::FromArgb(255, 10, 39, 66))
    $graphics.DrawImageUnscaled($bitmap, 0, 0)
    $previewDestination = Join-Path $outputPath ($file.BaseName + '_preview.png')
    $preview.Save($previewDestination, [Drawing.Imaging.ImageFormat]::Png)
    $graphics.Dispose()
    $preview.Dispose()
    $bitmap.Dispose()
    Write-Output $destination
    Write-Output $previewDestination
}
