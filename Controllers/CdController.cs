using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class CdController : Controller
    {
       private readonly ILogger<CdController> _logger;
        private readonly ApplicationDbContext _context;

        public CdController(ILogger<CdController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }

        public IActionResult Index()
        {
            // This will look for Views/About/Index.cshtml
          var CD = _context.Homwelcome.Where(x => x.LoanType == "CD").ToList();
            return View(CD);
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
