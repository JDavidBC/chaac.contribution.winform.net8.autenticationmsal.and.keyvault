using BabilaFuente.Services.Auth;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Data;


namespace BabilaFuente.Services;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IAuthService auth)
    {

        LoggingSetup.ConfigureLogging(configuration);

        // Registrar LoggerFactory de Serilog
        services.AddSingleton(new LoggerFactory().AddSerilog());

        // Permitir resolver ILogger<T> directamente
        services.AddLogging();

        // Otros servicios
        services.AddSingleton(configuration);

        //  services.AddSingleton<ISettingsService, SettingsService>();

        // Registrar la conexión a la base de datos como singleton o transient según diseño
        services.AddTransient<IDbConnection>(provider =>
        {
            var connectionString = auth.GetSecretAsync("SqlConnection").GetAwaiter().GetResult();
            return new SqlConnection(connectionString);
        });

        services.AddSingleton<Form1>(); // si quieres que siempre sea la misma ventana principal

        return services;
    }
}
