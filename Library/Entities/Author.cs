using System.ComponentModel.DataAnnotations;

namespace Library.Entities
{
    public class Author
    {
        [Key]   
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public ICollection<AuthorBook>? AuthorBooks { get; set; }
    }
}
