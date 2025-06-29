using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Pages
{
    public class LoginModel : PageModel
    {
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

        public IActionResult OnPost()
        {
            // Dummy check: Replace with real authentication logic
            if (Username == "admin" && Password == "password")
            {
                // Redirect to home or dashboard
                return RedirectToPage("/Movies/Index");
            }

            ErrorMessage = "Invalid username or password";
            ModelState.AddModelError(string.Empty, ErrorMessage);
            return Page();
        }
    }
}
