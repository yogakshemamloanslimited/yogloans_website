using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class InvestorController : Controller
    {
          private readonly ILogger<InvestorController> _logger;
        private readonly ApplicationDbContext _context;

        public InvestorController(ILogger<InvestorController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }


        [HttpGet]
        [Route("investor")]
        [Route("investor/index")]
        
        public async Task<IActionResult> Index()
        {   
              var investor = await _context.InvestorsWelcome.ToListAsync();
            return View(investor);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
