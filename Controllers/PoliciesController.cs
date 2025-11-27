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


          [HttpGet]
[Route("policy")]
public async Task<IActionResult> Policy()
{
    var Policy = await _context.Policy
          .Select(c => new
               {
                   c.Id,
                   c.Title,
                   // Convert byte[] to Base64 string if it exists
                   FileBase64 = c.FilePath != null ? Convert.ToBase64String(c.FilePath) : null
               })
               .ToListAsync();
            return Json(Policy); 
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
