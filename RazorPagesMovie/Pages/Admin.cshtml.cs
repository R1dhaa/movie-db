using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;


[Authorize(Roles = "Admin")]
public class AdminModel : PageModel
{
    public void OnGet()
    {
    }
}
