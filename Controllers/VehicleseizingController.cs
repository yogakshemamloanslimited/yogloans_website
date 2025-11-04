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


    
        //[HttpPost]
        //public JsonResult Setotp(string otp)
        //{
        //    if (int.TryParse(otp, out int otpValue))
        //    {
        //        HttpContext.Session.SetInt32("otp", otpValue);
        //        return Json(new { success = true, message = "OTP stored successfully" });
        //    }
        //    else
        //    {
        //        HttpContext.Session.SetString("otp", otp);
        //        return Json(new { success = true, message = "Non-numeric OTP stored successfully" });
        //    }
        //}



    }
}
