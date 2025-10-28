
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;


namespace yogloansdotnet.Controllers
{
    public class GrievanceController : Controller
    {
       

        public IActionResult Index()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");

            // If not logged in → redirect to Login
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login");
            }

            // Otherwise, proceed to the Index view
            return View();

        }
        public IActionResult Login()
        {
            return View("~/Views/Grievance/login.cshtml");
        }

    }
}
