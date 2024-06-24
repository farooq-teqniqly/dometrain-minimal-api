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

    public async Task<bool> UpdateBookAsync(Book book)
    {
        // Update is really a Delete/Add.
        var bookToDelete = await GetByIsbnAsync(book.Isbn);

        if (bookToDelete is null)
        {
            return false;
        }

        var bookToAdd = new Book
        {
            Author = book.Author,
            Isbn = book.Isbn,
            PageCount = book.PageCount,
            ReleaseDate = book.ReleaseDate,
            ShortDescription = book.ShortDescription,
            Title = book.Title
        };

        _database.Remove(bookToDelete);
        _database.Add(bookToAdd);

        return true;
    }

    public async Task<bool> DeleteBookAsync(string isbn)
    {
        var bookToDelete = await GetByIsbnAsync(isbn);

        if (bookToDelete is null)
        {
            return false;
        }

        _database.Remove(bookToDelete);

        return true;
    }

    public Task<Book?> GetByIsbnAsync(string isbn)
    {
        return Task.FromResult(_database.SingleOrDefault(
            book => string.Compare(
                isbn,
                book.Isbn,
                StringComparison.InvariantCultureIgnoreCase) == 0));
    }

    public Task<IEnumerable<Book>> GetAllAsync()
    {
        return Task.FromResult(_database.AsEnumerable());
    }

    public Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
    {
        return Task.FromResult(_database.Where(book =>
            book.Title.Contains(
                searchTerm,
                StringComparison.InvariantCultureIgnoreCase)));
    }
}
