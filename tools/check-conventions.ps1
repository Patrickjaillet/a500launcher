param(
    [string]$Repo = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = "Stop"
$issues = 0

function Fail([string]$message) {
    Write-Host "FAIL  $message"
    $script:issues++
}

$sourceDirs = @("Models", "Services", "I18n") | ForEach-Object { Join-Path $Repo $_ }
$rootSources = Get-ChildItem $Repo -Filter *.cs -File
$allSources = @($rootSources) + @($sourceDirs | Where-Object { Test-Path $_ } | ForEach-Object { Get-ChildItem $_ -Recurse -Filter *.cs -File })

foreach ($file in $allSources) {
    $lineNumber = 0
    foreach ($line in [System.IO.File]::ReadAllLines($file.FullName)) {
        $lineNumber++
        $code = $line
        $codePart = ($code -split '"')[0]
        if ($codePart -match '(^|[^:/])//' -and $codePart -notmatch 'https?://') {
            Fail "$($file.Name):$lineNumber comment (//)"
        }
        if ($codePart -match '/\*') {
            Fail "$($file.Name):$lineNumber comment (/* */)"
        }
    }
}

$frenchWords = @('\bpour\b', '\bavec\b', '\bfichier\b', '\bchemin\b', '\bréglage', '\bdisquette', '\bdémarr', '\butilisateur\b', '\bécran\b')
foreach ($file in $allSources) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $codeOnly = ($text -split '"') | Where-Object { $_ } | ForEach-Object { $_ } | Select-Object -Index (0..1000 | Where-Object { $_ % 2 -eq 0 })
    $joined = ($codeOnly -join ' ')
    foreach ($w in $frenchWords) {
        if ($joined -match $w) { Fail "$($file.Name) French identifier or code token matching $w" }
    }
}

function Test-Ignored([string]$path) {
    & git -C $Repo check-ignore -q -- $path
    return $LASTEXITCODE -eq 0
}

$mdFiles = Get-ChildItem $Repo -Filter *.md -File -Recurse |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|node_modules)\\' -and -not (Test-Ignored $_.FullName) }
$frenchMarkers = @('\bles\b', '\bune\b', '\best\b', '\bavec\b', '\bpour\b', '\bça\b', '\bréglages\b')
foreach ($file in $mdFiles) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $hits = 0
    foreach ($m in $frenchMarkers) { if ([regex]::Matches($text, $m).Count -gt 2) { $hits++ } }
    if ($hits -ge 3) { Fail "$($file.Name) looks like French prose (only ROADMAP.md may be French)" }
}

$forbidden = @('claude', 'anthropic', 'copilot', 'chatgpt', '\bgpt-')
$policyFiles = @('check-conventions.ps1', 'ci.yml')
$tracked = Get-ChildItem $Repo -Recurse -File |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|dist|\.git|node_modules)\\' -and $policyFiles -notcontains $_.Name }
foreach ($file in $tracked) {
    if ($file.Extension -notin '.cs', '.xaml', '.md', '.json', '.iss', '.isl', '.ps1', '.csproj', '.sln', '.props', '.yml', '.yaml', '.html', '.txt') { continue }
    if (Test-Ignored $file.FullName) { continue }
    $text = [System.IO.File]::ReadAllText($file.FullName)
    foreach ($p in $forbidden) {
        if ($text -match $p) { Fail "$($file.Name) contains forbidden marker '$p'" }
    }
}

$phaseWords = @('\bphase\s+\d', '\bphase\s+(one|two|three|four|five)\b', '-alpha', '-beta', '\balpha\s+release', '\bbeta\s+release')
$publicText = Get-ChildItem $Repo -Recurse -File -Include *.md, *.iss, *.isl, *.html |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|dist)\\' }
foreach ($file in $publicText) {
    if (Test-Ignored $file.FullName) { continue }
    $text = [System.IO.File]::ReadAllText($file.FullName).ToLower()
    foreach ($w in $phaseWords) {
        if ($text -match $w) { Fail "$($file.Name) uses a disallowed release-name form ($w)" }
    }
}

$gitignore = Join-Path $Repo ".gitignore"
$required = @('ROADMAP.md', 'CLAUDE.md', 'COMPILATION.md', 'GITHUB_DEPOT.md', '*.props', '*.zip')
if (Test-Path $gitignore) {
    $g = Get-Content $gitignore -Raw
    foreach ($entry in $required) {
        if ($g -notmatch [regex]::Escape($entry)) { Fail ".gitignore missing entry '$entry'" }
    }
}
else {
    Fail ".gitignore not found"
}

if ($issues -eq 0) {
    Write-Host "check-conventions: OK"
    exit 0
}

Write-Host "check-conventions: $issues issue(s)"
exit 1
