using BookLibrary.Data.Entities;

namespace BookLibrary.Data.Repositories;

public interface IBookRepository
{
    Task<bool> AddBook(Book book);
    Task<bool> UpdateBook(Book book);
    Task<bool> DeleteBook(string isbn);
}
