using BookLibrary.Data.Entities;
using BookLibrary.Data.Repositories;

namespace BookLibrary.Data.InMemory;
internal class BookRepository : IBookRepository, IReadOnlyBookRepository
{
    private static readonly List<Book> _database = [];

    public async Task<bool> AddBookAsync(Book book, CancellationToken ctx = default)
    {
        var existingBook = await GetByIsbnAsync(book.Isbn, ctx);

        if (existingBook is not null)
        {
            return false;
        }

        _database.Add(book);

        return true;
    }

    public async Task<bool> UpdateBookAsync(Book book, CancellationToken ctx = default)
    {
        // Update is really a Delete/Add.
        var bookToDelete = await GetByIsbnAsync(book.Isbn, ctx);

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

    public async Task<bool> DeleteBookAsync(string isbn, CancellationToken ctx = default)
    {
        var bookToDelete = await GetByIsbnAsync(isbn, ctx);

        if (bookToDelete is null)
        {
            return false;
        }

        _database.Remove(bookToDelete);

        return true;
    }

    public Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ctx = default)
    {
        return Task.FromResult(_database.SingleOrDefault(
            book => string.Compare(
                isbn,
                book.Isbn,
                StringComparison.InvariantCultureIgnoreCase) == 0));
    }

    public Task<IEnumerable<Book>> GetAllAsync(CancellationToken ctx = default)
    {
        return Task.FromResult(_database.AsEnumerable());
    }

    public Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm, CancellationToken ctx = default)
    {
        return Task.FromResult(_database.Where(book =>
            book.Title.Contains(
                searchTerm,
                StringComparison.InvariantCultureIgnoreCase)));
    }
}
