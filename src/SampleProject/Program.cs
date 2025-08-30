using Azure.Identity;
using BabilaFuente.Services;
using BabilaFuente.Services.Auth;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;



namespace BabilaFuente;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var auth = new AuthService(config);
        var scopes = new[] { "User.Read" };

        auth.SignOutAsync().GetAwaiter().GetResult();

        auth.EnsureSignedInAsync(scopes).GetAwaiter().GetResult();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<IAuthService>(auth) // ya logueado
            .AddApplicationServices(config, auth) // aquí registras IDbConnection, logging, etc.
            .BuildServiceProvider();

        ApplicationConfiguration.Initialize();
        Application.Run(services.GetRequiredService<Form1>());


    }
}