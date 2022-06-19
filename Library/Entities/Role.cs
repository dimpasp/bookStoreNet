using System.ComponentModel.DataAnnotations;

namespace Library.Entities
{
    public class Role
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string? RoleName { get; set; }
    }
}
