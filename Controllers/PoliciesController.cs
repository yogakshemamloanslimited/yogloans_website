using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class PoliciesController : Controller
    {
       private readonly ILogger<PoliciesController> _logger;
        private readonly ApplicationDbContext _context;

        public PoliciesController(ILogger<PoliciesController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }


  public async Task<IActionResult> Index()
        {
           var Policy = await _context.PolicyWelcome.ToListAsync();
            return View(Policy);
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
