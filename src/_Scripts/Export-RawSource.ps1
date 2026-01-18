<# 
.SYNOPSIS
  Builds a single "raw source" file by concatenating C# entity sources
  (enum/interface/class/record) from specified folders (recursively),
  grouped by folder category, into one output file.

.EXAMPLE
  From src\_Scripts folder:
  .\Export-RawSource.ps1 `
    -ProjectName "Portiforce.SimpleAssetAssistant.Core" `
    -RootPath "..\Core\Portiforce.SimpleAssetAssistant.Core"
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string] $ProjectName,

    [Parameter(Mandatory=$true)]
    [string] $RootPath,

    # Logical category folders to scan under $RootPath
   	# Core model: 
	[string[]] $CategoryFolders = @("Enums", "Interfaces", "Models", "Primitives", "Rules", "Extensions"),
	# Application model:
	#[string[]] $CategoryFolders = @("Entitlements", "Enums", "Exceptions", "Interfaces", "Models", "Responses", "Tech","UseCases"),

    # Output extension: txt, tzt, md, etc.
    [string] $OutExtension = "txt",

    # Optional: also include "struct"
    [switch] $IncludeStruct
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ---------------------------
# Config
# ---------------------------
$entityKeywords = @("enum","interface","class","record")
if ($IncludeStruct) { $entityKeywords += "struct" }

$excludedDirNames = @("bin","obj",".git",".vs",".idea","node_modules")

$entityNameRegex = [regex] '(?m)^\s*(?:\[[^\]]+\]\s*)*(?:public|internal|protected|private)?\s*(?:sealed\s+|static\s+|abstract\s+|partial\s+|readonly\s+)*\s*(?:record\s+)?(?:class|interface|enum|struct|record)\s+(?<name>@?[A-Za-z_][A-Za-z0-9_]*)\b'

# ---------------------------
# Helpers
# ---------------------------

function Resolve-FullPath([string]$path) {
    $base = $PSScriptRoot
    $combined = if ([System.IO.Path]::IsPathRooted($path)) { $path } else { Join-Path $base $path }
    return (Resolve-Path -LiteralPath $combined).Path
}

function Is-ExcludedPath([string]$fullPath) {
    foreach ($d in $excludedDirNames) {
        $needle = [System.IO.Path]::DirectorySeparatorChar + $d + [System.IO.Path]::DirectorySeparatorChar
        if ($fullPath -match [regex]::Escape($needle)) { return $true }
    }
    return $false
}

function File-ContainsEntity([string]$filePath, [string[]]$keywords) {
    $content = Get-Content -LiteralPath $filePath -Raw -Encoding UTF8
    foreach ($k in $keywords) {
        if ($content -match "(?i)\b$([regex]::Escape($k))\b") {
            return $true
        }
    }
    return $false
}

function Get-EntityNames([string]$fileContent) {
    $matches = $entityNameRegex.Matches($fileContent)
    if ($matches.Count -eq 0) { return @() }
    return $matches | ForEach-Object { $_.Groups["name"].Value } | Select-Object -Unique
}

# ---------------------------
# Paths
# ---------------------------

$rootFull = Resolve-FullPath $RootPath

if (-not (Test-Path -LiteralPath $rootFull)) {
    throw "RootPath does not exist: '$rootFull'"
}

# 👉 FIXED OUTPUT LOCATION (relative to script)
$outDirFull = Join-Path $PSScriptRoot "Artefacts\rawSource"
if (-not (Test-Path -LiteralPath $outDirFull)) {
    New-Item -ItemType Directory -Path $outDirFull -Force | Out-Null
}

$outFile = Join-Path $outDirFull ("{0}-rawSource.{1}" -f $ProjectName, $OutExtension)

# ---------------------------
# Collect files per category
# ---------------------------
$categoryToFiles = [ordered]@{}
foreach ($cat in $CategoryFolders) {
    $catPath = Join-Path $rootFull $cat
    if (-not (Test-Path -LiteralPath $catPath)) {
        $categoryToFiles[$cat] = @()
        continue
    }

    $files = Get-ChildItem -LiteralPath $catPath -Recurse -File -Filter *.cs |
        Where-Object { -not (Is-ExcludedPath $_.FullName) } |
        Sort-Object FullName

    $filtered = foreach ($f in $files) {
        if (File-ContainsEntity -filePath $f.FullName -keywords $entityKeywords) { $f }
    }

    $categoryToFiles[$cat] = @($filtered)
}

# ---------------------------
# Build output
# ---------------------------
$sb = New-Object System.Text.StringBuilder

$null = $sb.AppendLine("// Project: $ProjectName")
$null = $sb.AppendLine("// Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss K')")
$null = $sb.AppendLine("// Source root: $rootFull")
$null = $sb.AppendLine("// Output: $outFile")
$null = $sb.AppendLine()

$sectionIndex = 0

foreach ($cat in $CategoryFolders) {
    $sectionIndex++
    $files = @($categoryToFiles[$cat])

    $null = $sb.AppendLine("// $sectionIndex. $cat")
    $null = $sb.AppendLine()

    if ($files.Count -eq 0) {
        $null = $sb.AppendLine("// (no matching .cs files found)")
        $null = $sb.AppendLine()
        continue
    }

    $entityIndex = 0
    foreach ($f in $files) {
        $content = Get-Content -LiteralPath $f.FullName -Raw -Encoding UTF8
        $entityNames = @(Get-EntityNames $content)

        if ($entityNames.Count -eq 0) {
            $entityNames = @([System.IO.Path]::GetFileNameWithoutExtension($f.Name))
        }

        foreach ($name in $entityNames) {
            $entityIndex++
            $null = $sb.AppendLine("// $sectionIndex.$entityIndex $name")
        }

        $null = $sb.AppendLine("// Source: $($f.FullName)")
        $null = $sb.AppendLine($content.TrimEnd())
        $null = $sb.AppendLine()
        $null = $sb.AppendLine("// ------------------------------------------------------------")
        $null = $sb.AppendLine()
    }
}

[System.IO.File]::WriteAllText(
    $outFile,
    $sb.ToString(),
    (New-Object System.Text.UTF8Encoding($false))
)

Write-Host "✅ Raw source created:" -ForegroundColor Green
Write-Host "   $outFile"

Write-Host "`nStats:" -ForegroundColor Cyan
foreach ($cat in $CategoryFolders) {
    Write-Host (" - {0}: {1} file(s)" -f $cat, @($categoryToFiles[$cat]).Count)
}
