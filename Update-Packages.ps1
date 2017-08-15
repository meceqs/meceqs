$ErrorActionPreference = "Stop"

Get-ChildItem -Filter *.csproj -Recurse -Depth 10 | Foreach-Object {

    $projectPath = $_.FullName
    Write-Host "Processing $projectPath"

    $projectXml = [XML](Get-Content $projectPath)
    $projectXml.SelectNodes("/Project/ItemGroup/PackageReference") | Foreach-Object {

        $packageName = $_.Include

        Write-Host "- Package $packageName"

        dotnet add $projectPath package $packageName
        #if ($LASTEXITCODE -ne 0) { throw "error" }
    }
}