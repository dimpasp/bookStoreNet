using Library.Entities;

namespace Library.Interfaces
{
    public interface IAuthorRepository
    {
        Task<List<Author>> GetAll();
        Task<Author> GetAuthorById(int id);
        Task DeleteAuthor(int id);
        Task<Author> DetailsOfAuthor(int id);
        Task AddAuthor(Author author);
        Task UpdateAuthor(Author author);
    }
}
