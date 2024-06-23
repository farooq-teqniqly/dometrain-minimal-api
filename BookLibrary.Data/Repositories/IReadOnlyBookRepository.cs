using BookLibrary.Data.Entities;

namespace BookLibrary.Data.Repositories;

public interface IReadOnlyBookRepository
{
    Task<Book?> GetByIsbn(string isbn);
    Task<IEnumerable<Book>> GetAll();
    Task<IEnumerable<Book>> SearchByTitle(string searchTerm);
}
