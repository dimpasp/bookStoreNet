using System.ComponentModel.DataAnnotations;

namespace Library.Entities
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Phone]
        public int Phone { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? Age { get; set; }
        public DateTimeOffset UserDateCreated { get; set; }
        public ICollection<StudentBook>? StudentBooks{ get; set; }
        public ICollection<RatingStudentBook>? RatingStudentBooks { get; set; }

    }
}
