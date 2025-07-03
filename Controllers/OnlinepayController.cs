using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;

namespace yogloansdotnet.Controllers
{
    public class OnlinepayController : Controller
    {
        private readonly ILogger<OnlinepayController> _logger;

        public OnlinepayController(ILogger<OnlinepayController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string CustomerId, string mobile)
        {
           ViewBag.CustomerId = CustomerId;
           ViewBag.MobileNo = mobile;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
