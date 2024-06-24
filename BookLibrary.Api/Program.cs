using BookLibrary.Api.Auth;
using BookLibrary.Data.Entities;
using BookLibrary.Data.Extensions;
using BookLibrary.Data.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace BookLibrary.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddAuthentication(ApiKeySchemeConstants.SchemeName)
            .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(
                ApiKeySchemeConstants.SchemeName,
                _ => { });

        builder.Services.AddAuthorization();

        builder.Services.AddRepositories();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        AddSwaggerSupport(builder.Services);

        var app = builder.Build();

        app.Services.SeedInMemoryDatabase();

        UseSwagger(app, builder);

        app.UseAuthorization();

        app.MapPost(
            "books",
            [Authorize(AuthenticationSchemes = ApiKeySchemeConstants.SchemeName, Roles = "Admin")]
        async (
                Book book,
                IBookRepository bookRepository,
                IValidator<Book> validator,
                HttpContext httpContext) =>
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

                return Results.CreatedAtRoute(
                    RouteNames.GetBookByIsbn,
                    new { isbn = book.Isbn },
                    book);

            }).WithName(RouteNames.AddBook);

        app.MapGet(
            "books/{isbn}",
            async (
                string isbn,
                IReadOnlyBookRepository bookRepository,
                IValidator<string> isbnValidator) =>
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
        }).WithName(RouteNames.GetBookByIsbn);

        app.MapGet(
            "books",
            async (
                [FromQuery(Name = "q")] string? searchTerm,
                IReadOnlyBookRepository bookRepository) =>
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                var allBooks = await bookRepository.GetAllAsync();
                return Results.Ok(allBooks);
            }

            var filteredBooks = await bookRepository.SearchByTitleAsync(searchTerm);

            return Results.Ok(filteredBooks);
        }).WithName(RouteNames.SearchBooksByTitle);

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
            }).WithName(RouteNames.UpdateBook);

        app.MapDelete(
            "books",
            async (
                string isbn,
                IBookRepository bookRepository,
                IValidator<string> isbnValidator) =>
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
        }).WithName(RouteNames.DeleteBook);



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

        services.AddSwaggerGen(c =>
        {
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
    }

}
