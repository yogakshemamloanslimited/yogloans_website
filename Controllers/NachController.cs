using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;

namespace yogloansdotnet.Controllers
{
    public class NachController : Controller
    {
        private readonly ILogger<NachController> _logger;

        public NachController(ILogger<NachController> logger)
        {
            _logger = logger;
        }

        // This will respond to: /Online15g?CustomerId=0
        [HttpGet]
        public IActionResult Index(string CustomerId, string mobile)
        {
            ViewBag.CustomerId = CustomerId;
            ViewBag.mobile = mobile;
            return View("~/views/nach/index.cshtml");
        }


         public IActionResult pdf(string CustomerId)
        {
            ViewBag.CustomerId = CustomerId;
            return View("~/views/Online15g/pdf.cshtml");
        }
    }
}
