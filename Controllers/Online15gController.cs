using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;

namespace yogloansdotnet.Controllers
{
    public class Online15gController : Controller
    {
        private readonly ILogger<Online15gController> _logger;

        public Online15gController(ILogger<Online15gController> logger)
        {
            _logger = logger;
        }

        // This will respond to: /Online15g?CustomerId=0
        [HttpGet]
        public IActionResult Index(string CustomerId )
        {
            ViewBag.CustomerId = CustomerId;
            return View("~/views/Online15g/index.cshtml");
        }


  public IActionResult pdf(string customer_id, string pantrack, string pan, string j)
{
    ViewBag.CustomerId = customer_id;
    ViewBag.pantrack = pantrack;
    ViewBag.pan = pan;
    ViewBag.j = j;
    return View("~/views/Online15g/pdf.cshtml");
}


    }
}
