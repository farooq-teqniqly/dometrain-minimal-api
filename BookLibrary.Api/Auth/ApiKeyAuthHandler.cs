using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace BookLibrary.Api.Auth;

internal class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthSchemeOptions>
{
    [Obsolete]
    public ApiKeyAuthHandler(
        IOptionsMonitor<ApiKeyAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public ApiKeyAuthHandler(
        IOptionsMonitor<ApiKeyAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        const string invalidApiKeyMessage = "Invalid API key.";
        var failResult = AuthenticateResult.Fail(invalidApiKeyMessage);

        if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var headerValue))
        {
            return Task.FromResult(failResult);
        }

        var header = headerValue.ToString();

        if (header != Options.ApiKey)
        {
            return Task.FromResult(failResult);
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, "farooq@foo.com"),
            new Claim(ClaimTypes.Name, "Farooq"),
            new Claim(ClaimTypes.Role, "Admin"),
        };

        var authenticationType = "ApiKey";
        var claimsIdentity = new ClaimsIdentity(claims, authenticationType);

        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(claimsIdentity),
            Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}