using Library.Data;
using Library.Entities;
using Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Repositories
{
    public class AuthorRepository: IAuthorRepository
    {
        private readonly LibraryContext context;

        public AuthorRepository(LibraryContext context)
        {
            this.context = context;
        }

        public async Task<List<Author>> GetAll()
        {
            return await context.Author.ToListAsync();
        }

        public async Task<Author> GetAuthorById(int id)
        {
            return await context.Author.FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task DeleteAuthor(int id)
        {
            var author = await context.Author.FindAsync(id);
            context.Author.Remove(author);
            await context.SaveChangesAsync();
        }
        public async Task AddAuthor(Author author)
        {
            context.Add(author);
            await context.SaveChangesAsync();
        }
        public async Task UpdateAuthor(Author author)
        {
            context.Update(author);
            await context.SaveChangesAsync();
        }
        public async Task<Author> DetailsOfAuthor(int id)
        {
            var author = await context.Author
                                       .Include(x => x.AuthorBooks)
                                       .ThenInclude(x => x.Book)
                                       .FirstOrDefaultAsync(m => m.Id == id);
            return author;
        }
    }
}
