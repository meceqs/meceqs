using System;
using Meceqs.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.AspNetCore.Hosting
{
    /// <summary>
    /// Initializes Meceqs components on application startup.
    /// </summary>
    public class MeceqsStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                var meceqsInitializer = app.ApplicationServices.GetRequiredService<MeceqsInitializer>();
                meceqsInitializer.Start();
                next(app);
            };
        }
    }
}