$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$run = Join-Path $root ('validation\installer-ui-' + [guid]::NewGuid().ToString('N'))
$null = New-Item -ItemType Directory -Path $run
$target = Join-Path $run 'installation with spaces'
Add-Type -AssemblyName System.Drawing
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class InstallerUiProbe {
  [StructLayout(LayoutKind.Sequential)] public struct Rect { public int Left,Top,Right,Bottom; }
  [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out Rect r);
  [DllImport("user32.dll")] public static extern uint GetDpiForWindow(IntPtr h);
  [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr h, IntPtr dc, uint flags);
}
'@
$exe = Join-Path $root 'dist\RecFlareLauncher-Setup.exe'
# Exercise the actual visible one-click page, while keeping integration/launch off.
$proc = Start-Process -FilePath $exe -ArgumentList "/EXTRACTONLY /D=$target" -PassThru
$window = [IntPtr]::Zero
for ($i=0; $i -lt 100 -and $window -eq [IntPtr]::Zero; $i++) {
  Start-Sleep -Milliseconds 50
  $proc.Refresh()
  $window = $proc.MainWindowHandle
}
if ($window -eq [IntPtr]::Zero) { throw 'Visible installer window was not found' }
# The page callback finishes layout before payload extraction begins.
Start-Sleep -Milliseconds 150
$rect = New-Object InstallerUiProbe+Rect
$null = [InstallerUiProbe]::GetWindowRect($window, [ref]$rect)
$width = $rect.Right - $rect.Left
$height = $rect.Bottom - $rect.Top
$dpi = [InstallerUiProbe]::GetDpiForWindow($window)
$bitmap = New-Object System.Drawing.Bitmap($width, $height)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$dc = $graphics.GetHdc()
try { $null = [InstallerUiProbe]::PrintWindow($window, $dc, 2) }
finally { $graphics.ReleaseHdc($dc) }
$bitmap.Save((Join-Path $run 'installer.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose(); $bitmap.Dispose()
for ($i=0; $i -lt 120 -and -not $proc.HasExited; $i++) { $null = $proc.WaitForExit(500) }
$proc.Refresh()
if (-not $proc.HasExited -or $proc.ExitCode -ne 0) { throw 'Interactive extraction failed or timed out' }
if ([Math]::Abs($width - (400*$dpi/96)) -gt 4 -or [Math]::Abs($height - (160*$dpi/96)) -gt 4) { throw "Unexpected installer dimensions: $width x $height at $dpi DPI" }
if (-not (Test-Path -LiteralPath (Join-Path $target 'resources\app.asar'))) { throw 'Payload was not extracted' }
$launched = Get-CimInstance Win32_Process | Where-Object { $_.ExecutablePath -eq (Join-Path $target 'RecFlare Launcher.exe') }
if ($launched) { throw 'Extraction-only must not launch the app' }
$report = @{status='passed';width=$width;height=$height;dpi=$dpi;autoClosed=$true;extractionDidNotLaunch=$true;runRoot=$run}
$report | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $root 'validation\installer-ui-summary.json') -Encoding UTF8
$report | ConvertTo-Json
