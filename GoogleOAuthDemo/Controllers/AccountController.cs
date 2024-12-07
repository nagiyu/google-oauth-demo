using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GoogleOAuthDemo.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("Index", "Home");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, "Google");
        }
    }
}
