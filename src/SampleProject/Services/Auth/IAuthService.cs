using Microsoft.Identity.Client;

namespace BabilaFuente.Services.Auth;

public interface IAuthService
{
    Task<AuthenticationResult> EnsureSignedInAsync(string[] scopes);
    Task<string> GetAccessTokenAsync(string[] scopes);   // para pedir tokens después
    Task SignOutAsync();

    Task<string> GetSecretAsync(string secretName);

    string? CurrentUsername { get; }
    string? CurrentName { get; }
}
