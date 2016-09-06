Start-Process `
    -FilePath powershell.exe `
    -ArgumentList @( "dotnet run; Read-Host 'Press enter to exit'" ) `
    -WorkingDirectory "samples\Customers.Hosts.WebApi"

Start-Sleep -Seconds 5

Start-Process `
    -FilePath powershell.exe `
    -ArgumentList @( "dotnet run; Read-Host 'Press enter to exit'" ) `
    -WorkingDirectory "samples\Sales.Hosts.ProcessCustomerEvents"

Start-Sleep -Seconds 5

Start-Process `
    -FilePath powershell.exe `
    -ArgumentList @( "dotnet run; Read-Host 'Press enter to exit'" ) `
    -WorkingDirectory "samples\TrafficGenerator"