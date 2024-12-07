using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GoogleOAuthDemo.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("Index", "Home");
            System.IO.File.AppendAllText("output.log", $"{DateTime.Now} Redirect URI: {redirectUrl}\n");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, "Google");
        }
    }
}
