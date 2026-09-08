param(
    [string]$Repo = (Split-Path -Parent $PSScriptRoot),
    [string]$Version = '6030',
    [string]$ExpectedSha256 = 'F3E55700FD811CD543FDFEBF6C3221CFAA3D385828F110C6D3EB534C64503123'
)

$ErrorActionPreference = 'Stop'

$dir = Join-Path $Repo 'winuae'
$exe = Join-Path $dir 'winuae64.exe'

if (Test-Path $exe) {
    $have = (Get-FileHash $exe -Algorithm SHA256).Hash
    if ($have -eq $ExpectedSha256) {
        Write-Host "winuae64.exe already present and verified."
        exit 0
    }
    Write-Host "winuae64.exe present but hash differs; re-downloading."
    Remove-Item $exe -Force
}

New-Item -ItemType Directory -Force $dir | Out-Null
$zip = Join-Path $env:TEMP "WinUAE$Version`_x64.zip"
$url = "https://download.abime.net/winuae/releases/WinUAE$Version`_x64.zip"

Write-Host "Downloading $url"
Invoke-WebRequest -Uri $url -OutFile $zip -UseBasicParsing
Expand-Archive -Path $zip -DestinationPath $dir -Force
Remove-Item $zip -Force

$have = (Get-FileHash $exe -Algorithm SHA256).Hash
if ($have -ne $ExpectedSha256) {
    Write-Error "winuae64.exe SHA-256 mismatch: expected $ExpectedSha256, got $have"
    exit 1
}

Write-Host "winuae64.exe downloaded and verified."
