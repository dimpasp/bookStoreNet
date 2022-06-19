using Library.ViewModels.Author;
using Library.ViewModels.Rating;
using Library.ViewModels.Student;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Library.ViewModels.Book
{
    public class BookViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }
        [IntegerValidator]
        public int Pages { get; set; }
        public bool IsSelected { get; set; }
        [IntegerValidator]
        [Required(ErrorMessage = "Amount of books is required")]
        public int Amount { get; set; }
        [IntegerValidator]
        [Required(ErrorMessage = "Price is required")]
        public int Price { get; set; }
        public List<AuthorViewModel>? AuthorBooks { get; set; }
        public List<StudentViewModel>? RatingStudentBooks { get; set; }

        public int BookId { get;  set; }
        public bool BooksExists { get; set; }
        public int availableBooks { get; set; }

        public float? Rating { get; set; }
        public string? Comment { get; set; }
        public RatingSystemValues? ratingSystemValues { get; set; }
        public BookCategories? bookCategory { get; set; }
    }
}
