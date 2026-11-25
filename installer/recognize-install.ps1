param([Parameter(Mandatory=$true)][string]$InstallDirectory)
# Read metadata only. Never run an existing launcher or uninstaller.
$ErrorActionPreference = 'Stop'
try {
    $exe = if (Test-Path -LiteralPath (Join-Path $InstallDirectory 'RecFlare Launcher.exe')) { 'RecFlare Launcher.exe' } else { 'Rec Room Studio Launcher.exe' }
    foreach ($relative in @($exe, 'resources\app.asar', 'resources\config.json')) {
        $file = Get-Item -LiteralPath (Join-Path $InstallDirectory $relative)
        if ($file.PSIsContainer -or ($file.Attributes -band [IO.FileAttributes]::ReparsePoint)) { exit 87 }
    }
    $configPath = Join-Path $InstallDirectory 'resources\config.json'
    if ((Get-Item -LiteralPath $configPath).Length -gt 1MB) { exit 87 }
    $config = Get-Content -LiteralPath $configPath -Raw | ConvertFrom-Json
    if ($config.id -cne 'recroom-studio-launcher') { exit 87 }
    if ($config.name -cnotin @('Rec Room Studio Launcher', 'RecFlare Studio', 'RecFlare Studio Launcher')) { exit 87 }
    if ($config.version -cin @('2.0.0', '2.0.1')) { exit 0 }
    if ($config.version -ceq '3.0.0') {
        $manifestPath = Join-Path $InstallDirectory 'build-manifest.json'
        if ((Get-Item -LiteralPath $manifestPath).Length -gt 4MB) { exit 87 }
        $manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
        if ($manifest.version -ceq '3.0.0' -and $manifest.updater -ceq 'source-built-dotnet-rpc') { exit 0 }
    }
} catch { exit 87 }
exit 87
