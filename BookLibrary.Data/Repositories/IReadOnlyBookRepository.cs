using BookLibrary.Data.Entities;

namespace BookLibrary.Data.Repositories;

public interface IReadOnlyBookRepository
{
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ctx = default);
    Task<IEnumerable<Book>> GetAllAsync(CancellationToken ctx = default);
    Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm, CancellationToken ctx = default);
}
