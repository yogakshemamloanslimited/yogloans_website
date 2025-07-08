using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class GoldController : Controller
    {
        private readonly ILogger<GoldController> _logger;
        private readonly ApplicationDbContext _context;

        public GoldController(ILogger<GoldController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }
       public async Task<IActionResult> Index()
        {
            var goldLoans = _context.Homwelcome.Where(x => x.LoanType == "Gold").ToList();
            return View(goldLoans);
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
