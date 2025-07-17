using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class CareerController : Controller
    {
        private readonly ILogger<CareerController> _logger;
        private readonly ApplicationDbContext _context;

        public CareerController(ILogger<CareerController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }


         public async Task<IActionResult> Index()
        {
          var career = await _context.CareerWelcome.ToListAsync();
            return View(career);
        }
      [HttpGet]
[Route("departments")]
public async Task<IActionResult> Departments()
{
    var department = await _context.Career.Where(x => x.Status == "1").ToListAsync();
    return Json(department);
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
