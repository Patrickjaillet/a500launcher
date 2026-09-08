param(
    [string]$Dir = (Join-Path $PSScriptRoot "..\assets\i18n")
)

$ErrorActionPreference = "Stop"

$reference = Join-Path $Dir "en.json"
if (-not (Test-Path $reference)) {
    Write-Error "Reference file not found: $reference"
    exit 2
}

function Read-Keys([string]$path) {
    $json = Get-Content $path -Raw | ConvertFrom-Json
    $json.PSObject.Properties.Name | Sort-Object
}

$refKeys = Read-Keys $reference
$placeholder = [regex]"\{(\d+)\}"

$issues = 0

Get-ChildItem $Dir -Filter *.json | Where-Object { $_.Name -ne "en.json" } | ForEach-Object {
    $name = $_.Name
    $keys = Read-Keys $_.FullName

    $missing = $refKeys | Where-Object { $keys -notcontains $_ }
    $extra = $keys | Where-Object { $refKeys -notcontains $_ }

    foreach ($k in $missing) { Write-Host "$name : missing key '$k'"; $issues++ }
    foreach ($k in $extra) { Write-Host "$name : unknown key '$k'"; $issues++ }

    $refJson = Get-Content $reference -Raw | ConvertFrom-Json
    $locJson = Get-Content $_.FullName -Raw | ConvertFrom-Json
    foreach ($k in $refKeys) {
        if ($keys -notcontains $k) { continue }
        $refSlots = ($placeholder.Matches([string]$refJson.$k) | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
        $locSlots = ($placeholder.Matches([string]$locJson.$k) | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
        if (($refSlots -join ",") -ne ($locSlots -join ",")) {
            Write-Host "$name : placeholder mismatch on '$k' (expected {$($refSlots -join ',')}, got {$($locSlots -join ',')})"
            $issues++
        }
    }
}

if ($issues -eq 0) {
    Write-Host "i18n-check: OK"
    exit 0
}

Write-Host "i18n-check: $issues issue(s)"
exit 1
