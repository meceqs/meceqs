[CmdletBinding(PositionalBinding = $false)]
param(
    [string] $ArtifactsPath = (Join-Path $PWD "artifacts"),
    [string] $BuildConfiguration = "Release",

    [bool] $RunBuild = $true,
    [bool] $RunTests = $true
)

$ErrorActionPreference = "Stop"
$Stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

function Task {
    [CmdletBinding()] param (
        [Parameter(Mandatory = $true)] [string] $name,
        [Parameter(Mandatory = $false)] [bool] $runTask,
        [Parameter(Mandatory = $false)] [scriptblock] $cmd
    )

    if ($cmd -eq $null) {
        throw "Command is missing for task '$name'. Make sure the starting '{' is on the same line as the term 'Task'. E.g. 'Task `"$name`" `$Run$name {'"
    }

    if ($runTask -eq $true) {
        Write-Host "`n------------------------- [$name] -------------------------`n" -ForegroundColor Cyan
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        & $cmd
        Write-Host "`nTask '$name' finished in $($sw.Elapsed.TotalSeconds) sec."
    }
    else {
        Write-Host "`n------------------ Skipping task '$name' ------------------" -ForegroundColor Yellow
    }
}

Task "install-dotnet" $true {
    $path = $PSScriptRoot

    # Ensures that .net core is up to date.
    # first get the required version from global.json
    $json = ConvertFrom-Json (Get-Content "$path/global.json" -Raw)
    $required_version = $json.sdk.version

    # Running dotnet --version stupidly fails if the required SDK version is higher
    # than the currently installed version. So move global.json out the way
    # and then put it back again
    Rename-Item "$path/global.json" "$path/global.json.bak"
    $current_version = (dotnet --version)
    Rename-Item "$path/global.json.bak" "$path/global.json"
    Write-Host "Required .NET version: $required_version Installed: $current_version"

    if ($current_version -lt $required_version) {
        # Current installed version is too low.
        # Install new version as a local only dependency.
        $urlCurrent = "https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$required_version/dotnet-sdk-$required_version-win-x64.zip"
        Write-Host "Installing .NET Core $required_version from $urlCurrent"
        $env:DOTNET_INSTALL_DIR = "$path/.dotnetsdk"
        New-Item -Type Directory $env:DOTNET_INSTALL_DIR -Force | Out-Null
        (New-Object System.Net.WebClient).DownloadFile($urlCurrent, "dotnet.zip")
        Write-Host "Unzipping to $env:DOTNET_INSTALL_DIR"
        Add-Type -AssemblyName System.IO.Compression.FileSystem; [System.IO.Compression.ZipFile]::ExtractToDirectory("dotnet.zip", $env:DOTNET_INSTALL_DIR)

        $env:PATH = "$env:DOTNET_INSTALL_DIR;$env:PATH"
    }
}


Task "Init" $true {

    if ($ArtifactsPath -eq $null) { "Property 'ArtifactsPath' may not be null." }
    if ($BuildConfiguration -eq $null) { throw "Property 'BuildConfiguration' may not be null." }
    if ((Get-Command "dotnet" -ErrorAction SilentlyContinue) -eq $null) { throw "'dotnet' command not found. Is .NET Core SDK installed?" }

    Write-Host "ArtifactsPath: $ArtifactsPath"
    Write-Host "BuildConfiguration: $BuildConfiguration"
    Write-Host ".NET Core SDK: $(dotnet --version)`n"

    Remove-Item -Path $ArtifactsPath -Recurse -Force -ErrorAction Ignore
    New-Item $ArtifactsPath -ItemType Directory -ErrorAction Ignore | Out-Null
    Write-Host "Created artifacts folder '$ArtifactsPath'"
}

Task "Build" $RunBuild {

    dotnet msbuild "/t:Restore;Build;Pack" "/p:CI=true" `
        "/p:Configuration=$BuildConfiguration" `
        "/p:PackageOutputPath=$(Join-Path $ArtifactsPath "nuget")"

    if ($LASTEXITCODE -ne 0) { throw "Build failed." }
}

Task "Tests" $RunTests {

    $testOutput = Join-Path $ArtifactsPath "Tests"
    New-Item $testOutput -ItemType Directory -ErrorAction Ignore | Out-Null

    $testsFailed = $false
    Get-ChildItem -Filter *.csproj -Recurse | ForEach-Object {

        if (Select-Xml -Path $_.FullName -XPath "/Project/ItemGroup/PackageReference[@Include='Microsoft.NET.Test.Sdk']") {
            $library = Split-Path $_.DirectoryName -Leaf
            $testResultOutput = Join-Path $testOutput "$library.trx"

            dotnet test $_.FullName -c $BuildConfiguration --no-build --logger "trx;LogFileName=$testResultOutput"
            if ($LASTEXITCODE -ne 0) { $testsFailed = $true }
        }
    }

    if ($testsFailed) { throw "At least one test failed." }
}

Write-Host "`nBuild finished in $($Stopwatch.Elapsed.TotalSeconds) sec." -ForegroundColor Green
