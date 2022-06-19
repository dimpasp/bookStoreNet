using Library.Data;
using Library.Entities;
using Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext context;

        public BookRepository(LibraryContext context)
        {
            this.context = context;
        }
        public IEnumerable<Book> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return context.Book;
            }
            return context.Book.Where(e => e.Title.Contains(searchTerm));
        }

        public async Task<Book?> GetById(int id)
        {
            return await context.Book
                                .Include(x => x.AuthorBooks)
                                .ThenInclude(x => x.Author)
                                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<List<Book>> GetAllBooks()
        {
            return await context.Book.ToListAsync();
        }

        public async Task<bool> CheckIfBookExist(int id)
        {
            return await context.Book.FirstOrDefaultAsync(x => x.Id == id) == null ? false : true;
        }
        public async Task<float?> GetRatingOfBook(int id)
        {
            return context.RatingStudentBook.Where(x => x.BookId == id).Select(x => x.Rating).Average();
        }
        public int HowManyTimesBookExist(int id)
        {
            return context.StudentBook
                    .Where(b => b.BookId == id)
                    .Select(x => x.Id)
                    .Count();
        }
        public async Task<RatingStudentBook> checkIfUserHaveRate(int studentId,int bookId)
        {
            return await context.RatingStudentBook.FirstOrDefaultAsync(g => g.StudentId == studentId && g.BookId == bookId);
        }
        public async Task CreateBook(Book book)
        {
            context.Add(book);
            await context.SaveChangesAsync();
        }
        public async Task CreateRatingStudentBook(RatingStudentBook ratingStudentBook)
        {
            context.Add(ratingStudentBook);
            await context.SaveChangesAsync();
        }
        public async Task UpdateRatingStudentBook(RatingStudentBook ratingStudentBook)
        {
            context.Update(ratingStudentBook);
            await context.SaveChangesAsync();
        }
        public async Task<bool> AddBook(int id,Student student)
        {
            if (student.StudentBooks?.FirstOrDefault(x => x.BookId == id) is StudentBook)
                return false;

            var existBook = await CheckIfBookExist(id);
            if (existBook == false)
                return false;

            student.StudentBooks.Add(new StudentBook
            {
                BookId = id
            });

            var result = await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveBook(int id, Student student)
        {
            var bookToRemove = context.StudentBook.Where(x => x.BookId == id && x.StudentId == student.Id).FirstOrDefault();
            if (bookToRemove == null)
                return false;
            student.StudentBooks.Remove(bookToRemove);

            var result = await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteBook(int id)
        {
            try
            {
                var book = await context.Book.FindAsync(id);
                var authorsBooks = context.AuthorBook.Where(m => m.BookId == id);
                if (authorsBooks != null)
                {
                    foreach (var item in authorsBooks)
                    {
                        context.AuthorBook.Remove(item);
                    }
                }
                context.Book.Remove(book);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public List<RatingStudentBook> GetStudentBooksRatings(int id)
        {
            return context.RatingStudentBook.Where(m => m.BookId == id).ToList();
        }
    }
}
