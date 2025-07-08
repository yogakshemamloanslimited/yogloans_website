using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class VehicleController : Controller
    {
        private readonly ILogger<VehicleController> _logger;
        private readonly ApplicationDbContext _context;

        public VehicleController(ILogger<VehicleController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }


        public IActionResult Index()
        {
            // This will look for Views/About/Index.cshtml
           var Vehicle = _context.Homwelcome.Where(x => x.LoanType == "Vehicle").ToList();
            return View(Vehicle);
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
