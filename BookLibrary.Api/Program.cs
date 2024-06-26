using System.Net;
using BookLibrary.Api.Auth;
using BookLibrary.Api.Extensions;
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

        AddApiKeyAuthentication(builder.Services);

        builder.Services.AddRepositories();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        AddSwaggerSupport(builder.Services);

        var app = builder.Build();

        app.Services.SeedInMemoryDatabase();

        UseSwagger(app, builder);

        app.UseAuthorization();

        app.MapPost(
                "v1/books",
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

                }).WithName(RouteNames.AddBook)
            .AcceptsJson<Book>()
            .ProducesUnauthorizedResponse()
            .ProducesCreatedResponse<Book>()
            .ProducesBadRequestResponse()
            .WithDefaultTags();

        app.MapGet(
                "v1/books/{isbn}",
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
                }).WithName(RouteNames.GetBookByIsbn)
            .ProducesOkResponse<Book>()
            .ProducesNotFoundResponse()
            .ProducesBadRequestResponse()
            .WithDefaultTags();

        app.MapGet(
                "v1/books",
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
                }).WithName(RouteNames.SearchBooksByTitle)
            .ProducesOkResponse<IEnumerable<Book>>()
            .WithDefaultTags();

        app.MapPut(
            "v1/books/{isbn}",
            [Authorize(AuthenticationSchemes = ApiKeySchemeConstants.SchemeName, Roles = "Admin")]
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
            }).WithName(RouteNames.UpdateBook)
            .AcceptsJson<Book>()
            .ProducesUnauthorizedResponse()
            .ProducesOkResponse<Book>()
            .ProducesBadRequestResponse()
            .WithDefaultTags();

        app.MapDelete(
            "v1/books",
            [Authorize(AuthenticationSchemes = ApiKeySchemeConstants.SchemeName, Roles = "Admin")]

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
        }).WithName(RouteNames.DeleteBook)
            .ProducesUnauthorizedResponse()
            .ProducesNoContentResponse()
            .ProducesBadRequestResponse()
            .WithDefaultTags();

        app.MapGet("v1/ping", () => Results.Extensions.Html($@"<!doctype html>
            <html>
                <head>
                    <title>Book Library API Status Page</title>
                </head>
                <body>
                    <h2>The API is alive.</h2>
                    <p>{DateTime.Now}</p>
                </body>
            </html>"));

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
    }

    private static void AddApiKeyAuthentication(IServiceCollection services)
    {
        services
            .AddAuthentication(ApiKeySchemeConstants.SchemeName)
            .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(
                ApiKeySchemeConstants.SchemeName,
                _ => { });

        services.AddAuthorization();
    }

}
