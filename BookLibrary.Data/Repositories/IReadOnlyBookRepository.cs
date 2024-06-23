using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLibrary.Data.Entities;

namespace BookLibrary.Data.Repositories;

public interface IReadOnlyBookRepository
{
    Task<Book?> GetByIsbn(string isbn);
    Task<IEnumerable<Book>> GetAll();
    Task<IEnumerable<Book>> SearchByTitle(string searchTerm);
}
