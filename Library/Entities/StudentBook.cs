using System.ComponentModel.DataAnnotations;

namespace Library.Entities
{
    public class StudentBook
    {
        [Key]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int BookId { get; set; }
        

        public Book? Book { get; set; }
        public Student? Student { get; set; }
    }
}
