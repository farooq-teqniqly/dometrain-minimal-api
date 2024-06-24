using BookLibrary.Data.Entities;
using BookLibrary.Data.InMemory;
using BookLibrary.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BookLibrary.Data.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IBookRepository, BookRepository>();
        services.AddSingleton<IReadOnlyBookRepository, BookRepository>();

        return services;
    }
}

public static class ServiceProviderExtensions
{
    public static IServiceProvider SeedInMemoryDatabase(this IServiceProvider serviceProvider)
    {
        var bookRepository = serviceProvider.GetRequiredService<IBookRepository>();

        var books = new List<Book>
        {
            new()
            {
                Isbn = "978-0138203283",
                Title = "Clean Architecture with .NET",
                Author = "Dino Esposito",
                PageCount = 336,
                ReleaseDate = new DateOnly(2024, 3, 22),
                ShortDescription = "Understand what to do at any point in developing a clean .NET architecture"
            },
            new()
            {
                Isbn = "978-1492078005",
                Title = "Head First Design Patterns",
                Author = "Eric Freeman",
                PageCount = 669,
                ReleaseDate = new DateOnly(2021, 1, 12),
                ShortDescription = "Join hundreds of thousands of developers who've improved their object-oriented design skills through Head First Design Patterns."
            }
        };

        foreach (var book in books)
        {
            var bookAdded = bookRepository.AddBookAsync(book).Wait(TimeSpan.FromMilliseconds(100));

            if (!bookAdded)
            {
                throw new InvalidOperationException(
                    "Error seeding database. The book could not be added within the timeout period.");
            }
        }

        return serviceProvider;
    }
}
