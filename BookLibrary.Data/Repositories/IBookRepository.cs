using BookLibrary.Data.Entities;

namespace BookLibrary.Data.Repositories;

public interface IBookRepository
{
    Task<bool> AddBookAsync(Book book, CancellationToken ctx = default);
    Task<bool> UpdateBookAsync(Book book, CancellationToken ctx = default);
    Task<bool> DeleteBookAsync(string isbn, CancellationToken ctx = default);
}
