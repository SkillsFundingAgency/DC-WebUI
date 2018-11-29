using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Action(nameof(PostSignIn), "Account");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                WsFederationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult PostSignIn()
        {
            return RedirectToAction("Index", "SubmissionOptionsAuthorised");
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);

            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme,
                WsFederationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Index", "SubmissionOptionsAuthorised");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}