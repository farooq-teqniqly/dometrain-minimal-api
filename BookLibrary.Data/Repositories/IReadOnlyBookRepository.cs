using BookLibrary.Data.Entities;

namespace BookLibrary.Data.Repositories;

public interface IReadOnlyBookRepository
{
    Task<Book?> GetByIsbnAsync(string isbn);
    Task<IEnumerable<Book>> GetAllAsync();
    Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm);
}
