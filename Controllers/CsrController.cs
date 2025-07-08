using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class CsrController : Controller
    {
        private readonly ILogger<CsrController> _logger;
        private readonly ApplicationDbContext _context;

        public CsrController(ILogger<CsrController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }

      

         public async Task<IActionResult> Index()
        {
          
              var csr = await _context.CsrWelcome.ToListAsync();
            return View(csr);
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
