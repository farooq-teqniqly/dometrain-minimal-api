using Microsoft.AspNetCore.Authentication;

namespace BookLibrary.Api.Auth;

internal class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = "very-secret-key";
}