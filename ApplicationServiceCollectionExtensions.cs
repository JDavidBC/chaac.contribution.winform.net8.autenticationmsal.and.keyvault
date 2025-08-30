using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BabilaFuente;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register your application services here, for example:
        // services.AddTransient<IMyService, MyService>();
        // You can use the configuration parameter as needed.

        return services;
    }
}
