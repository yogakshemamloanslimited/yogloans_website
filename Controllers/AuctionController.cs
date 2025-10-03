using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace yogloansdotnet.Controllers
{
    public class AuctionController : Controller
    {
        private readonly ILogger<AuctionController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuctionController(ILogger<AuctionController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            // Check if session exists
            var codeValue = HttpContext.Session.GetInt32("code");

            if (codeValue.HasValue && codeValue.Value == 1)
            {
                var vm = new LoanGroupViewModel
                {
                    Gold = _context.Homwelcome.Where(x => x.LoanType == "Gold").ToList(),
                    Business = _context.Homwelcome.Where(x => x.LoanType == "Business").ToList(),
                    Vehicle = _context.Homwelcome.Where(x => x.LoanType == "Vehicle").ToList(),
                    CD = _context.Homwelcome.Where(x => x.LoanType == "CD").ToList(),
                    AboutContent = _context.AboutContent.ToList()
                };

                return View(vm);
            }

            // Redirect to login if session code is not 2
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View("~/Views/Auction/Login.cshtml");
        }

        public IActionResult Welcome()
        {


            return View("~/Views/Auction/welcome.cshtml");
        }


        [HttpPost]
        public IActionResult Logout()
        {

            HttpContext.Session.SetInt32("code", 3);



            return Json(new { success = true, message = "Logged out successfully" });
        }
    }

}
