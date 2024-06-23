using BookLibrary.Data.Entities;
using BookLibrary.Data.Repositories;

namespace BookLibrary.Data.InMemory;
internal class BookRepository : IBookRepository, IReadOnlyBookRepository
{
    private static readonly List<Book> _database = [];

    public Task<bool> AddBookAsync(Book book)
    {
        _database.Add(book);

        return Task.FromResult(true);
    }

    public Task<bool> UpdateBookAsync(Book book)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteBookAsync(string isbn)
    {
        throw new NotImplementedException();
    }

    public Task<Book?> GetByIsbnAsync(string isbn)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Book>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }
}
