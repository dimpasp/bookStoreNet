using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Identity
{
    public class LoginViewModel
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? UserName { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
