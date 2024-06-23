using BookLibrary.Data.Entities;
using BookLibrary.Data.Extensions;
using BookLibrary.Data.Repositories;

namespace BookLibrary.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRepositories();

        AddSwaggerSupport(builder.Services);

        var app = builder.Build();

        app.MapPost("books", async (Book book, IBookRepository bookRepository) =>
        {
            var added = await bookRepository.AddBook(book);

            if (!added)
            {
                return Results.BadRequest($"A book with ISBN '{book.Isbn}' already exists.");
            }

            return Results.Created($"/books/{book.Isbn}", book);
        });

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
