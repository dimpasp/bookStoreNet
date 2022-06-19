using System.ComponentModel.DataAnnotations;

namespace Library.Entities
{
    public class AuthorBook
    {
        [Key]
        public int Id { get; set; }
        public int BookId { get; set; }
        public int AuthorId { get; set; }

        public Book? Book { get; set; }
        public Author? Author { get; set; }
    }
}
