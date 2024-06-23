using BookLibrary.Data.Entities;
using BookLibrary.Data.Repositories;

namespace BookLibrary.Data.InMemory;
internal class BookRepository : IBookRepository, IReadOnlyBookRepository
{
    private static readonly List<Book> _database = [];

    public Task<bool> AddBook(Book book)
    {
        _database.Add(book);

        return Task.FromResult(true);
    }

    public Task<bool> UpdateBook(Book book)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteBook(string isbn)
    {
        throw new NotImplementedException();
    }

    public Task<Book?> GetByIsbn(string isbn)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Book>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Book>> SearchByTitle(string searchTerm)
    {
        throw new NotImplementedException();
    }
}
