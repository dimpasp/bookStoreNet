using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Library.ViewModels.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Library.Views.User
{
    public class LoginRazorModel : PageModel
    {
        public RegisterViewModel? LoginViewModel { get; set; }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid) return Page(); 

            if(LoginViewModel?.Email == "admin@admin.com" && LoginViewModel.Password =="password")
            {
                //security context
                var claims = new List<Claim> { new Claim(ClaimTypes.Email, "admin@admin.com") };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                return RedirectToPage("/Index");
            }
            return Page();
        }
    }

}
