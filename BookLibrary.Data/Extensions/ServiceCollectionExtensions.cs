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
