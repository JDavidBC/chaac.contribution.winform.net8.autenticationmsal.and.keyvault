using BabilaFuente.Services.Auth;
using Microsoft.Extensions.Logging;
using System.Data;

namespace BabilaFuente;

public partial class Form1 : Form
{
    private readonly ILogger<Form1> _logger;
    private readonly IDbConnection _connection;
    private readonly IAuthService _authService;

    public Form1(ILogger<Form1> logger, IDbConnection connection, IAuthService authService)
    {
        InitializeComponent();
        _logger = logger;
        _connection = connection;
        _authService = authService;


        _logger.LogInformation("Form1 initialized, user {0}", _authService.CurrentUsername);



    }
}
