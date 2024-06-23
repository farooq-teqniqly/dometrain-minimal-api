using BookLibrary.Data.Entities;

namespace BookLibrary.Data.Repositories;

public interface IBookRepository
{
    Task<bool> AddBookAsync(Book book);
    Task<bool> UpdateBookAsync(Book book);
    Task<bool> DeleteBookAsync(string isbn);
}
