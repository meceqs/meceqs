dotnet build
if ($LASTEXITCODE -ne 0) { throw "build error" }

Start-Process `
    -FilePath powershell.exe `
    -ArgumentList @( "dotnet run --no-restore; Read-Host 'Press enter to exit'" ) `
    -WorkingDirectory "samples\Customers.Hosts.WebApi"

Start-Sleep -Seconds 3

Start-Process `
    -FilePath powershell.exe `
    -ArgumentList @( "dotnet run --no-restore; Read-Host 'Press enter to exit'" ) `
    -WorkingDirectory "samples\Sales.Hosts.ProcessCustomerEvents"

Start-Sleep -Seconds 3

Start-Process `
    -FilePath powershell.exe `
    -ArgumentList @( "dotnet run --no-restore; Read-Host 'Press enter to exit'" ) `
    -WorkingDirectory "samples\Sales.Hosts.ProcessOrders"

Start-Sleep -Seconds 3

Start-Process `
    -FilePath powershell.exe `
    -ArgumentList @( "dotnet run --no-restore; Read-Host 'Press enter to exit'" ) `
    -WorkingDirectory "samples\TrafficGenerator"
