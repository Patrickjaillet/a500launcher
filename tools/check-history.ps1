param(
    [string]$Repo = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = "Stop"

$vendors = @(
    (-join @('c', 'l', 'a', 'u', 'd', 'e')),
    (-join @('a', 'n', 't', 'h', 'r', 'o', 'p', 'i', 'c')),
    (-join @('c', 'o', 'p', 'i', 'l', 'o', 't')),
    (-join @('c', 'h', 'a', 't', 'g', 'p', 't'))
)
$pattern = '(?i)(' + ($vendors -join '|') + ')'

$commits = git -C $Repo log --all --format='%H | %an <%ae> | %s'
$hits = $commits | Select-String -Pattern $pattern

$coauthors = git -C $Repo log --all --format='%B' | Select-String -Pattern '(?i)^co-authored-by:'

if ($hits -or $coauthors) {
    Write-Host "History references a disallowed assistant or carries a co-author trailer:"
    $hits
    $coauthors
    exit 1
}

Write-Host "check-history: OK"
exit 0
