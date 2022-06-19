using Library.Entities;
using Library.ViewModels.Book;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Author
{
    public class AuthorViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        public bool IsSelected { get; set; }
        public IEnumerable<BookViewModel>? AuthorBooks { get; set; }
    }
}
