using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace MyApplication.Areas.MicrosoftIdentity.Pages.Account;

public class SignedOutModel : PageModel
{
    public IActionResult OnGet()
    {
        return Redirect("/account/signed-out");
    }
}
