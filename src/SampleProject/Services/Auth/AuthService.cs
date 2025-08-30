using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using BabilaFuente.Services.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;

public class AuthService : IAuthService
{
    private readonly IPublicClientApplication _pca;
    private IAccount? _account;
    private AuthenticationResult? _lastAuth;
    private readonly string _vaultUrl;

    public AuthService(IConfiguration cfg)
    {
        var tenantId = cfg["AzureAd:TenantId"]!;
        var clientId = cfg["AzureAd:ClientId"]!;
        var redirect = cfg["AzureAd:RedirectUri"] ?? "http://localhost";
        _vaultUrl = cfg["KeyVault:Url"] ?? throw new ArgumentNullException("KeyVault:Url");

        _pca = PublicClientApplicationBuilder.Create(clientId)
            .WithTenantId(tenantId)
            .WithRedirectUri(redirect)
            .Build();


    }

    // ----------------------
    // Autenticación general
    // ----------------------
    public async Task<AuthenticationResult> EnsureSignedInAsync(string[] scopes)
    {
        _account ??= (await _pca.GetAccountsAsync()).FirstOrDefault();

        try
        {
            _lastAuth = await _pca.AcquireTokenSilent(scopes, _account)
                                   .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            _lastAuth = await _pca.AcquireTokenInteractive(scopes)
                                   .WithPrompt(Prompt.SelectAccount)
                                   .ExecuteAsync();
        }

        _account = _lastAuth.Account;
        return _lastAuth;
    }

    public async Task<string> GetAccessTokenAsync(string[] scopes)
    {
        _account ??= (await _pca.GetAccountsAsync()).FirstOrDefault();

        try
        {
            var res = await _pca.AcquireTokenSilent(scopes, _account)
                                .ExecuteAsync();
            _lastAuth = res;
            _account = res.Account;
            return res.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            var res = await _pca.AcquireTokenInteractive(scopes)
                                .WithAccount(_account)
                                .ExecuteAsync();
            _lastAuth = res;
            _account = res.Account;
            return res.AccessToken;
        }
    }

    public async Task SignOutAsync()
    {
        var accounts = await _pca.GetAccountsAsync();
        foreach (var acc in accounts)
            await _pca.RemoveAsync(acc);
        _account = null;
        _lastAuth = null;
    }

    // ----------------------
    // Info del usuario logueado
    // ----------------------
    public string? CurrentUsername => _lastAuth?.Account?.Username;

    public string? CurrentName
    {
        get
        {
            if (_lastAuth?.IdToken is null) return null;
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(_lastAuth.IdToken);
            return jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value
                ?? jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        }
    }

    // ----------------------
    // Obtener secretos desde Key Vault
    // ----------------------
    public async Task<string> GetSecretAsync(string secretName)
    {
        _account ??= (await _pca.GetAccountsAsync()).FirstOrDefault();

        // Scope correcto para Key Vault
        var kvScopes = new[] { $"{_vaultUrl}/.default" };

        AuthenticationResult result;
        try
        {
            result = await _pca.AcquireTokenSilent(kvScopes, _account)
                               .ExecuteAsync();

             _lastAuth = result;
        }
        catch (MsalUiRequiredException)
        {
            // Consentimiento interactivo solo si es necesario
            result = await _pca.AcquireTokenInteractive(kvScopes)
                               .WithAccount(_account)
                               .ExecuteAsync();


            _lastAuth = result;
        }

        // Adapter para SecretClient
        var cred = new AccessTokenCredential(result.AccessToken);
        var client = new SecretClient(new Uri(_vaultUrl), cred);
        _lastAuth = result;

        KeyVaultSecret secret = await client.GetSecretAsync(secretName);
        return secret.Value;
    }

    // ----------------------
    // Adaptador para SecretClient
    // ----------------------
    public class AccessTokenCredential : TokenCredential
    {
        private readonly string _accessToken;
        public AccessTokenCredential(string accessToken) => _accessToken = accessToken;

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
            new AccessToken(_accessToken, DateTimeOffset.UtcNow.AddMinutes(30));

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
            new ValueTask<AccessToken>(new AccessToken(_accessToken, DateTimeOffset.UtcNow.AddMinutes(30)));
    }
}
