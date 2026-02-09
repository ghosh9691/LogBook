using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LogBook.Web.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool ValidateCredentials(string username, string password)
    {
        var configuredUsername = _configuration["LOGBOOK_USERNAME"]
            ?? Environment.GetEnvironmentVariable("LOGBOOK_USERNAME")
            ?? "admin";
        var configuredPassword = _configuration["LOGBOOK_PASSWORD"]
            ?? Environment.GetEnvironmentVariable("LOGBOOK_PASSWORD")
            ?? "admin";
        Console.WriteLine($"Configured Username: {configuredUsername}, Configured Password: {configuredPassword}");
        return string.Equals(username, configuredUsername, StringComparison.OrdinalIgnoreCase)
            && password == configuredPassword;
    }

    public async Task SignInAsync(string username)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, "User")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    public async Task SignOutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }

    public string? GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.Name;
    }
}
