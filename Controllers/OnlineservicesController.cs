using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class OnlineservicesController : Controller
    {
         private readonly ILogger<OnlineservicesController> _logger;
        private readonly ApplicationDbContext _context;

        public OnlineservicesController(ILogger<OnlineservicesController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }

               public async Task<IActionResult> Index()
        {
            var online = await _context.OnlineWelcome.ToListAsync();
            return View(online);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
        [Route("payonline")]
        public IActionResult Payonline(string CustomerId, string mobile)
        {
            ViewBag.CustomerId = CustomerId;
            ViewBag.mobile = mobile;
            return View("~/Views/Payonline/index.cshtml", ViewBag);
        }
    }
}
