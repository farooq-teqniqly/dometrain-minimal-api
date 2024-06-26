using BookLibrary.Api.Auth;
using Microsoft.OpenApi.Models;

namespace BookLibrary.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(ApiKeySchemeConstants.SchemeName)
            .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(
                ApiKeySchemeConstants.SchemeName,
                _ => { });

        services.AddAuthorization();

        return services;
    }

    internal static IServiceCollection AddSwaggerMiddleware(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Book Library API", Version = "v1" });

            c.AddSecurityDefinition(ApiKeySchemeConstants.SchemeName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "API Key Authentication",
                Scheme = ApiKeySchemeConstants.SchemeName
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = ApiKeySchemeConstants.SchemeName
                        }
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }
}