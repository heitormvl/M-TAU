using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace M_TAU.Client.Auth;

public sealed class TokenAuthenticationStateProvider(IJSRuntime js) : AuthenticationStateProvider
{
    private const string TokenKey = "authToken";
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetTokenAsync();
        return string.IsNullOrWhiteSpace(token) ? Anonymous : BuildAuthState(token);
    }

    public async Task NotifyUserAuthenticatedAsync(string token)
    {
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        NotifyAuthenticationStateChanged(Task.FromResult(BuildAuthState(token)));
    }

    public async Task NotifyUserLoggedOutAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    public async Task<string?> GetTokenAsync()
        => await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

    private static AuthenticationState BuildAuthState(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var segments = jwt.Split('.');
        if (segments.Length < 2) return [];

        var payload = segments[1].Replace('-', '+').Replace('_', '/');
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        var claims = new List<Claim>();
        using var doc = JsonDocument.Parse(json);
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            // Map standard JWT claim names to .NET ClaimTypes
            var type = prop.Name switch
            {
                "sub" => ClaimTypes.NameIdentifier,
                "email" => ClaimTypes.Email,
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" => ClaimTypes.Role,
                _ => prop.Name
            };

            if (prop.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in prop.Value.EnumerateArray())
                    claims.Add(new Claim(type, item.ToString()));
            }
            else
            {
                claims.Add(new Claim(type, prop.Value.ToString()));
            }
        }

        return claims;
    }
}
