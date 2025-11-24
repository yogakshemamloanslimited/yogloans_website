using Microsoft.AspNetCore.Mvc;

namespace yogloansdotnet.Controllers
{
    public class VehicleseizingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Application_Id( string application_id)
        {
            try
            {
                if (string.IsNullOrEmpty(application_id))
                    return Json(new { success = false, message = "CustomerId is missing." });

                // ✅ Make sure no nulls are passed to SetString
                application_id = application_id ?? string.Empty;

                HttpContext.Session.SetString("application_id", application_id);
           

                return Json(new { success = true, message = "Session data set successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult Productdetails()
        {
            return View();
        }
        [HttpPost]
        public IActionResult product_details(string application_id , string pagestate)
        {
            try
            {




                HttpContext.Session.SetString("pagestate", pagestate);

                HttpContext.Session.SetString("application_id", application_id);


                return Json(new { success = true, message = "Session data set successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
