namespace BookLibrary.Api.Extensions;

internal static class WebApplicationExtensions
{
    internal static void UseSwaggerMiddleware(this WebApplication webApplication)
    {
        webApplication.UseSwagger();

        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseSwaggerUI();
        }
    }
}