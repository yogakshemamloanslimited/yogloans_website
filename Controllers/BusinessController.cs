using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class BusinessController : Controller
    {
       private readonly ILogger<BusinessController> _logger;
        private readonly ApplicationDbContext _context;

        public BusinessController(ILogger<BusinessController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }
       public async Task<IActionResult> Index()
        {
            // This will look for Views/About/Index.cshtml
            var BusinessLoan = _context.Homwelcome.Where(x => x.LoanType == "Business").ToList();
            return View(BusinessLoan);
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
