using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Role
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string? RoleName { get; set; }
    }
}
