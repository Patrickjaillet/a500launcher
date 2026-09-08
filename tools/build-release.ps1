param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$repo = Split-Path -Parent $PSScriptRoot
Push-Location $repo

try {
    $csproj = Join-Path $repo "A500Launcher.csproj"

    $described = (& git -C $repo describe --tags --abbrev=0 2>$null)
    if ($LASTEXITCODE -eq 0 -and $described) {
        $version = $described.TrimStart("v")
    }
    else {
        $version = ([xml](Get-Content $csproj)).Project.PropertyGroup.Version
    }
    if (-not $version) { throw "Version could not be determined." }
    Write-Host "Building A500 Launcher $version"

    $tfm = "net8.0-windows10.0.22000.0"
    $publishDir = Join-Path $repo "bin\$Configuration\$tfm\publish"
    $distDir = Join-Path $repo "dist"

    if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }
    New-Item -ItemType Directory -Force $distDir | Out-Null

    dotnet test (Join-Path $repo "A500Launcher.sln") -c $Configuration --nologo
    if ($LASTEXITCODE -ne 0) { throw "Tests failed." }

    & (Join-Path $PSScriptRoot "check-conventions.ps1")
    if ($LASTEXITCODE -ne 0) { throw "check-conventions failed." }

    & (Join-Path $PSScriptRoot "check-history.ps1")
    if ($LASTEXITCODE -ne 0) { throw "check-history failed." }

    & (Join-Path $PSScriptRoot "i18n-check.ps1")
    if ($LASTEXITCODE -ne 0) { throw "i18n-check failed." }

    & (Join-Path $PSScriptRoot "fetch-winuae.ps1")
    if ($LASTEXITCODE -ne 0) { throw "fetch-winuae failed." }

    dotnet publish $csproj -c $Configuration -r win-x64 --self-contained false `
        -p:PublishSingleFile=false -o $publishDir --nologo
    if ($LASTEXITCODE -ne 0) { throw "Publish failed." }

    # Portable zip
    $zip = Join-Path $distDir "A500Launcher-$version-portable.zip"
    if (Test-Path $zip) { Remove-Item $zip -Force }
    Compress-Archive -Path (Join-Path $publishDir "*") -DestinationPath $zip
    Write-Host "Portable: $zip"

    # Installer
    $iscc = @(
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles(x86)}\Inno Setup 7\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 7\ISCC.exe"
    ) | Where-Object { Test-Path $_ } | Select-Object -First 1

    if (-not $iscc) {
        Write-Warning "Inno Setup not found; skipping installer."
    }
    else {
        & $iscc "/DAppVersion=$version" "/DPublishDir=$publishDir" (Join-Path $repo "installer\A500Launcher.iss")
        if ($LASTEXITCODE -ne 0) { throw "Installer build failed." }
        Write-Host "Installer: $(Join-Path $distDir "A500Launcher-Setup-$version.exe")"
    }

    # Checksums
    $sums = Join-Path $distDir "SHA256SUMS"
    Get-ChildItem $distDir -File | Where-Object { $_.Name -ne "SHA256SUMS" } | ForEach-Object {
        "$((Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLower())  $($_.Name)"
    } | Set-Content $sums -Encoding ascii
    Write-Host "Checksums: $sums"
}
finally {
    Pop-Location
}
