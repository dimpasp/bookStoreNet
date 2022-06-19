using Library.ViewModels.Book;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Store
{
    public class StoreViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        public bool IsSelected { get; set; }
        [Required(ErrorMessage = "PostalCode is required")]
        [MinLength(5, ErrorMessage = "Postal Code have 5 digits"), MaxLength(5, ErrorMessage = "Postal Code have 5 digits")]
        public string? PostalCode { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }
        public List<BookViewModel>? StoreBooks { get; set; }
    }
}
