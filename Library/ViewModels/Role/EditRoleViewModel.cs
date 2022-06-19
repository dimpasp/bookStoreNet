using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Role
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
        public String? Id{ get; set; }
        [Required(ErrorMessage ="Role name is required.")]
        public string? RoleName{ get; set; }
                        
        public List<string> Users{ get; set; }
    }
}
