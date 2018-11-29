using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "SubmissionOptionsAuthorised");
            }

            return View();
        }
    }
}