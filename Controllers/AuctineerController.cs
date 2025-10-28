using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;



namespace yogloansdotnet.Controllers
{
    public class AuctineerController : Controller
    {
        private readonly ILogger<AuctineerController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuctineerController(ILogger<AuctineerController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var codeValue = HttpContext.Session.GetInt32("code");

            if (codeValue.HasValue && codeValue.Value == 2)
            {
                var vm = new LoanGroupViewModel();
               

                return View(vm);
            }

            // Redirect to login if session code is not 2
            return RedirectToAction("Login");
        }
    }
}
