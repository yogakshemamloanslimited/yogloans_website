using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class BranchController : Controller
    {
          private readonly ApplicationDbContext _context;
            private readonly ILogger<BranchController> _logger;

        public BranchController(ILogger<BranchController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

     public async Task<IActionResult> Index()
        {
            var Nearby = await _context.Nearby.ToListAsync();
            return View(Nearby);
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
