using Library.ViewModels.Book;

namespace Library.Interfaces
{
    public interface IBookService
    {
        Task<BookViewModel> GetBookById(int id);
        Task<BookViewModel> GetEditBookView(int id);
        List<RatingPairs> AddStudentBookRating(int id);
    }
}
