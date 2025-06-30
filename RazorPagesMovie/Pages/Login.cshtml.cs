using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace RazorPagesMovie.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        [Required]
        public string Username { get; set; } = "";

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(Username, Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Redirect after successful login
                return RedirectToPage("/Movies/Index");
            }
            else
            {
                ErrorMessage = "Invalid username or password";
                ModelState.AddModelError(string.Empty, ErrorMessage);
                return Page();
            }
        }
    }
}
