using Library.ViewModels.Book;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }
        [Required]
        public int Amount{ get; set; }

        public int Pages { get; set; }
        public int Price { get; set; }

        public ICollection<AuthorBook>? AuthorBooks { get; set; }
        public ICollection<StudentBook>? StudentBooks{ get; set; }
        public ICollection<StoreBook>? StoreBooks { get; set; }
        public ICollection<RatingStudentBook>? RatingStudentBooks { get; set; }
        public BookCategories? bookCategory { get; set; }
    }
}

public enum BookCategories
{
    Action,
    Classics,
    Comic,
    Mystery,
    Fantasy,
    Horror,
    Novel,
    Poems
}