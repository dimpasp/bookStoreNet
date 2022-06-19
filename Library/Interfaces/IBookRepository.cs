using Library.Entities;

namespace Library.Interfaces
{
    public interface IBookRepository
    {
        IEnumerable<Book> Search(string searchTerm);
        Task<Book?> GetById(int id);
        Task<bool> CheckIfBookExist(int id);
        Task<bool> AddBook(int id, Student student);
        Task<bool> RemoveBook(int id, Student student);
        Task<bool> DeleteBook(int id);
        List<RatingStudentBook> GetStudentBooksRatings(int id);
        Task<List<Book>> GetAllBooks();
        Task CreateBook(Book book);
        Task CreateRatingStudentBook(RatingStudentBook ratingStudentBook);
        Task UpdateRatingStudentBook(RatingStudentBook ratingStudentBook);
        Task<RatingStudentBook> checkIfUserHaveRate(int studentId, int bookId);
        Task<float?> GetRatingOfBook(int id);
        int HowManyTimesBookExist(int id);
    }
}
