using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HPlusSport.Security.Identity.Pages
{
    [Authorize]
    public class PriviledgedModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
