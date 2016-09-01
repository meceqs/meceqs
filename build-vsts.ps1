Import-Module .\tools\psake\psake.psm1
Invoke-psake .\build-definition.ps1 -properties @{
    BuildNumber = $env:BUILD_BUILDNUMBER;
    BuildConfiguration = $env:BuildConfiguration;
    ArtifactsPath = $env:BUILD_ARTIFACTSTAGINGDIRECTORY 
}