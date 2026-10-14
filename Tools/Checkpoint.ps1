
<#
.SYNOPSIS
    Creates a verified Git checkpoint for AgentAvenUnity.

.DESCRIPTION
    WHAT:
    Reviews, stages, commits and pushes project changes.

    HOW:
    Validates the repository, branch and origin before
    staging approved paths and requesting confirmation.

    WHY:
    Provides a repeatable checkpoint workflow without
    accidentally pushing development work to main.

    DESIGN:
    Developer automation / guarded Git workflow.

    NOTE:
    Unity tests are currently verified manually.
    DryRun never modifies the Git index or repository.
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$Message,

    [switch]$DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

#region Configuration

# The script lives under <ProjectRoot>/Tools/.
$RepoRoot = (Resolve-Path (
    Join-Path $PSScriptRoot ".."
)).Path

$ExpectedRemotes = @(
    "https://github.com/DorManDel/AgentAvenUnity.git",
    "https://github.com/DorManDel/AgentAvenUnity",
    "git@github.com:DorManDel/AgentAvenUnity.git"
)

# Only these project locations may be staged automatically.
$AllowedPaths = @(
    "Assets",
    "Packages",
    "ProjectSettings",
    "Tools",
    "Docs",
    ".editorconfig",
    ".gitattributes",
    ".gitignore",
    ".vsconfig",
    "README.md",
    "LICENSE"
)

#endregion

#region Git Helper

<#
    Executes Git in the project root.
    Stops the script if the Git command fails.
#>
function Invoke-Git
{
    param(
        [string[]]$Arguments
    )

    & git -C $RepoRoot --no-pager @Arguments

    if ($LASTEXITCODE -ne 0)
    {
        throw "Git command failed: git $($Arguments -join ' ')"
    }
}

#endregion

#region Repository Validation

Write-Host "`n[Checkpoint] AgentAvenUnity" -ForegroundColor Cyan

$GitRoot = (
    Invoke-Git -Arguments @("rev-parse", "--show-toplevel")
).Trim()

if (
    [System.IO.Path]::GetFullPath($GitRoot) -ne
    [System.IO.Path]::GetFullPath($RepoRoot)
)
{
    throw "Script is not running in the expected repository."
}

$Branch = (
    Invoke-Git -Arguments @("branch", "--show-current")
).Trim()

if ([string]::IsNullOrWhiteSpace($Branch))
{
    throw "Detached HEAD is not allowed."
}

if ($Branch -in @("main", "master"))
{
    throw "Checkpoint blocked: protected branch '$Branch'."
}

$Origin = (
    Invoke-Git -Arguments @("remote", "get-url", "origin")
).Trim()

if ($Origin -notin $ExpectedRemotes)
{
    throw "Unexpected origin remote: $Origin"
}

Write-Host "Branch: $Branch" -ForegroundColor Green
Write-Host "Remote: $Origin"

#endregion

#region Preview

Write-Host "`n[Checkpoint] Current changes:" -ForegroundColor Cyan

Invoke-Git -Arguments @(
    "status",
    "--short",
    "--untracked-files=all"
)

if ($DryRun)
{
    Write-Host "`n[DRY RUN] No files staged, committed or pushed." `
        -ForegroundColor Yellow

    return
}

#endregion

#region Stage Approved Project Files

$PathsToStage = @()

foreach ($Path in $AllowedPaths)
{
    $AbsolutePath = Join-Path $RepoRoot $Path

    # A deleted tracked file still needs to be staged.
    $Tracked = @(
        Invoke-Git -Arguments @("ls-files", "--", $Path)
    )

    if ((Test-Path -LiteralPath $AbsolutePath) -or $Tracked.Count -gt 0)
    {
        $PathsToStage += $Path
    }
}

if ($PathsToStage.Count -eq 0)
{
    throw "No approved project paths were found."
}

Invoke-Git -Arguments (
    @("add", "-A", "--") + $PathsToStage
)

$Staged = @(
    Invoke-Git -Arguments @("diff", "--cached", "--name-only")
)

if ($Staged.Count -eq 0)
{
    Write-Host "[Checkpoint] No changes to commit." `
        -ForegroundColor Yellow
    return
}

#endregion

#region Safety Review

# Reject previously staged files outside approved project paths.
$Unexpected = @(
    $Staged | Where-Object {
        $_ -notmatch "^(Assets|Packages|ProjectSettings|Tools|Docs)/" -and
        $_ -notin @(
            ".editorconfig",
            ".gitattributes",
            ".gitignore",
            ".vsconfig",
            "README.md",
            "LICENSE"
        )
    }
)

if ($Unexpected.Count -gt 0)
{
    throw "Unexpected staged files: $($Unexpected -join ', ')"
}

# Block common secret or private-key filenames.
$Sensitive = @(
    $Staged | Where-Object {
        $_ -match '(?i)(^|/)\.env($|\.)' -or
        $_ -match '(?i)(secret|credential|private[_-]?key)' -or
        $_ -match '(?i)\.(pem|pfx|p12|key)$'
    }
)

if ($Sensitive.Count -gt 0)
{
    throw "Potentially sensitive staged files: $($Sensitive -join ', ')"
}

Write-Host "`n[Checkpoint] Staged files:" -ForegroundColor Cyan

$Staged | ForEach-Object {
    Write-Host "  $_"
}

Write-Host "`n[Checkpoint] Summary:" -ForegroundColor Cyan

Invoke-Git -Arguments @("diff", "--cached", "--stat")

#endregion

#region Confirmation

Write-Host "`nBefore committing:" -ForegroundColor Yellow
Write-Host "- Save your Unity scenes and files."
Write-Host "- Confirm compilation and relevant tests passed."
Write-Host "- Check that staged assets are permitted in the repository."

$Confirmation = Read-Host "Type CHECKPOINT to commit and push"

if ($Confirmation -cne "CHECKPOINT")
{
    Write-Host "Cancelled. Staged changes remain for review." `
        -ForegroundColor Yellow
    return
}

#endregion

#region Commit and Push

Write-Host "`n[Checkpoint] Creating commit..." -ForegroundColor Cyan

Invoke-Git -Arguments @("commit", "-m", $Message)

Write-Host "[Checkpoint] Pushing to $Branch..." -ForegroundColor Cyan

# Explicitly push HEAD to the same branch name on origin.
Invoke-Git -Arguments @(
    "push",
    "-u",
    "origin",
    "HEAD:refs/heads/$Branch"
)

$Hash = (
    Invoke-Git -Arguments @("rev-parse", "--short", "HEAD")
).Trim()

Write-Host "`nCheckpoint complete!" -ForegroundColor Green
Write-Host "Branch: $Branch"
Write-Host "Commit: $Hash"

Invoke-Git -Arguments @("status", "-sb")

#endregion
