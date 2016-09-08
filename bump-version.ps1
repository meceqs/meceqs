param (
    [switch]$Major,
    [switch]$Minor,
    [switch]$Patch
)

#########################
# Settings

# The current version will be read from this file
$projectFile = "src\Meceqs\project.json"

# All projects and dependencies starting with this name will be updated.
$packagePrefix = "Meceqs"


#########################
# Validate switches

$selectedSwitches = 0
if ($Major -eq $true) { $selectedSwitches++ }
if ($Minor -eq $true) { $selectedSwitches++ }
if ($Patch -eq $true) { $selectedSwitches++ } 

if ($selectedSwitches -eq 0) {
    throw "No version part selected for bump"
}
if ($selectedSwitches -gt 1) {
    throw "Only one version part can be selected for bump"
}

#########################
# Get current version from main project

$projectFileJson = Get-Content -Path $projectFile -Raw | ConvertFrom-Json -ErrorAction Ignore

$currentVersion = $projectFileJson.version

$versionParts = $currentVersion.TrimEnd("*-").Split(".")
$currentMajor = [Convert]::ToInt32($versionParts[0])
$currentMinor = [Convert]::ToInt32($versionParts[1])
$currentPatch = [Convert]::ToInt32($versionParts[2])

Write-Output "Current version: $currentVersion (Major: $currentMajor Minor: $currentMinor Patch: $currentPatch)"

#########################
# Bump version

if ($Major -eq $true) {
    $currentMajor++
    $currentMinor = 0
    $currentPatch = 0
}
if ($Minor -eq $true) {
    $currentMinor++
    $currentPatch = 0
}
if ($Patch -eq $true) {
    $currentPatch++
}

$newVersion = "$currentMajor.$currentMinor.$currentPatch-*"

Write-Output "New version: $newVersion"

#########################
# Update all library versions and dependencies in all projects

# The PowerShell JSON indentation is incredibly ugly and it reformats the whole document.
# That's why we use simple string replace for the actual file update.

Get-ChildItem -Filter project.json -Recurse -Depth 5 | ForEach-Object {
    Write-Output ("Processing " + $_.DirectoryName)

    $fileChanged = $false
    $fileContent = Get-Content -Path $_.FullName -Raw 
    $json = $fileContent | ConvertFrom-Json -ErrorAction Ignore

    # Should we update the library version?
    if ($_.Directory.Name.StartsWith($packagePrefix) -eq $true -and $json.version -ne $null) {
        
        $fileContent = $fileContent.Replace("""version"": ""$currentVersion""", """version"": ""$newVersion""")
        $fileChanged = $true

        Write-Output " - Changed version"
    }

    if ($json.dependencies -ne $null) {
        $json.dependencies `
            | Get-Member -MemberType NoteProperty `
            | Where-Object { $_.Name.StartsWith($packagePrefix) } `
            | Foreach-Object {
                $name = $_.Name

                $fileContent = $fileContent.Replace("""$name"": ""$currentVersion""", """$name"": ""$newVersion""")
                $fileChanged = $true

                Write-Output "  - Updated dependency '$name'"
            }
    }

    if ($fileChanged -eq $true) {
        $fileContent | Out-File $_.FullName
    }
}