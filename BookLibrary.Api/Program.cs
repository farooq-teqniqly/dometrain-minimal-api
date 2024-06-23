using BookLibrary.Data.Entities;
using BookLibrary.Data.Extensions;
using BookLibrary.Data.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace BookLibrary.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRepositories();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        AddSwaggerSupport(builder.Services);

        var app = builder.Build();

        app.MapPost(
            "books",
            async (
                Book book,
                IBookRepository bookRepository,
                IValidator<Book> validator) =>
            {
                var validationResult = await validator.ValidateAsync(book);

                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }

                var added = await bookRepository.AddBook(book);

                if (!added)
                {
                    return Results.BadRequest(new List<ValidationFailure>
                    {
                        new("Isbn", $"A book with ISBN '{book.Isbn}' already exists.")
                    });
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
