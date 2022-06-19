using Library.ViewModels.Book;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Student
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        [Phone]
        [Required(ErrorMessage = "Phone is required,only digits")]
        public int Phone { get; set; }
        public string? Gender { get; set; }
        [EmailAddress]
        [Required(ErrorMessage ="Student must have email")]
        public string? Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        [Required(ErrorMessage = "Student must have an age.")]
        [Range(15, 60, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? Age { get; set; }
        public bool IsSelected { get; set; }
        public List<int>? availableBooks { get; set; }

        public List<BookViewModel>? StudentBooks { get; set; }
        public StudentGender? studentGender { get; set; }

    }
}
