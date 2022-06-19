using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Identity
{
    public class RegisterViewModel
    {
        public int UserId{ get; set; }

        public string? Username { get; set; }
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage ="Password should be the same as Confirm Password")]
        public string? ConfirmPassword { get; set; }

    }
}
