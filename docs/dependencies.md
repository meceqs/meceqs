# Dependencies

### Target frameworks
Wherever possible, the libraries target [.NET Standard](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) to support
as many platforms as possible. Unfortunately, a few libraries require dependencies that are not yet available for .NET Standard. 
For this reason, they target only the full .NET Framework.

* __netstandard1.3:__ .NET Core 1.0, .NET Framework 4.6, Mono/Xamarin, Universal Windows Platform 10.0
  * `Meceqs`
  * `Meceqs.Serialization.Json`
* __netstandard1.5, net46:__ .NET Core 1.0, .NET Framework 4.6, Mono/Xamarin
  * `Meceqs.AspNetCore`: 
* __net46:__ .NET Framework 4.6
  * `Meceqs.AzureEventHubs`
  * `Meceqs.AzureEventHubs.FileFake`
  * `Meceqs.AzureServiceBus`
  * `Meceqs.AzureServiceBus.FileFake`

### Microsoft.Extensions.DependencyInjection.Abstractions 
Meceqs uses dependency injection extensively. Instead of defining its own contracts, Meceqs supports the new 
[dependency injection abstraction library](https://docs.asp.net/en/latest/fundamentals/dependency-injection.html)
from Microsoft. This allows you to use any container that supports this standard.

### Microsoft.Extensions.Logging.Abstractions
Similar to dependency injection, Meceqs uses the new [logging abstraction library](https://docs.asp.net/en/latest/fundamentals/logging.html) 
from Microsoft for creating log messages. This allows you to either use the official logger from Microsoft or to 
integrate any logging framework that supports this standard.

### Microsoft.Extensions.Options
This library is used to allow you to combine multiple configuration operations on simple POCO settings objects.