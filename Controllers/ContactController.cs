using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class ContactController : Controller
    {
      private readonly ILogger<ContactController> _logger;
        private readonly ApplicationDbContext _context;

        public ContactController(ILogger<ContactController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }
     public async Task<IActionResult> Index()
        {
          
               var ContactWelcome = await _context.ContactWelcome.ToListAsync();
            return View(ContactWelcome);
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
