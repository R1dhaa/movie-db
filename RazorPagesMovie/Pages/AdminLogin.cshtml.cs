using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Pages
{
    public class AdminLoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminLoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty, Required]
        public string Username { get; set; } = "";

        [BindProperty, Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByNameAsync(Username);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ErrorMessage = "Invalid admin credentials.";
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(Username, Password, false, false);
            if (result.Succeeded)
                return RedirectToPage("/Movies/Index");
                ErrorMessage = "Login failed.";
                return Page();
        }
    }
}
