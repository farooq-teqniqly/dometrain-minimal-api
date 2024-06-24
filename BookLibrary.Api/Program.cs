using BookLibrary.Data.Entities;
using BookLibrary.Data.Extensions;
using BookLibrary.Data.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

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

        app.Services.SeedInMemoryDatabase();

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

                var added = await bookRepository.AddBookAsync(book);

                if (!added)
                {
                    return Results.BadRequest(new List<ValidationFailure>
                    {
                        new("Isbn", $"A book with ISBN '{book.Isbn}' already exists.")
                    });
                }

                return Results.Created($"/books/{book.Isbn}", book);
            });

        app.MapGet("books/{isbn}", async (string isbn, IReadOnlyBookRepository bookRepository, IValidator<string> isbnValidator) =>
        {
            var validationResult = await isbnValidator.ValidateAsync(isbn);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var book = await bookRepository.GetByIsbnAsync(isbn);

            if (book is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(book);
        });

        app.MapGet("books", async ([FromQuery(Name = "q")] string? searchTerm, IReadOnlyBookRepository bookRepository) =>
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                var allBooks = await bookRepository.GetAllAsync();
                return Results.Ok(allBooks);
            }

            var filteredBooks = await bookRepository.SearchByTitleAsync(searchTerm);

            return Results.Ok(filteredBooks);
        });

        app.MapPut(
            "books/{isbn}",
            async (
                string isbn,
                Book book,
                IBookRepository bookRepository,
                IValidator<Book> validator) =>
            {
                book.Isbn = isbn;

                var validationResult = await validator.ValidateAsync(book);

                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }

                var updated = await bookRepository.UpdateBookAsync(book);

                if (!updated)
                {
                    return Results.NotFound();
                }

                return Results.Ok(book);
            });

        app.MapDelete("books", async (string isbn, IBookRepository bookRepository, IValidator<string> isbnValidator) =>
        {
            var validationResult = await isbnValidator.ValidateAsync(isbn);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var deleted = await bookRepository.DeleteBookAsync(isbn);

            if (deleted)
            {
                return Results.NoContent();
            }

            return Results.NotFound();
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
