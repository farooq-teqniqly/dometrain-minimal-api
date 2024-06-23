namespace BookLibrary.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AddSwaggerSupport(builder.Services);

        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        UseSwagger(app, builder);

        app.Run();
    }

    private static void UseSwagger(
        IApplicationBuilder app,
        WebApplicationBuilder builder)
    {
        app.UseSwagger();

        if (builder.Environment.IsDevelopment())
        {
            app.UseSwaggerUI();
        }
    }

    private static void AddSwaggerSupport(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}
