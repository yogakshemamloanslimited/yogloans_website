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
   var InvestorsWelcome = await _context.InvestorsWelcome.ToListAsync();
   var Aboutcontent = await _context.AboutContent.ToListAsync();
    var Directors = await _context.Directors.ToListAsync();
    var Leaders = await _context.Leaders.ToListAsync();
             var viewModel = new InvestoreViewModel
    {
        InvestorsWelcome = InvestorsWelcome ,
        AboutContent = Aboutcontent,
        Directors = Directors,
        Leaders = Leaders
    };

    return View(viewModel);
        }

        [HttpGet]
[Route("annual-report")]
public async Task<IActionResult> AnnualReport()
{
    var report = await _context.AnnualReports.ToListAsync();
    return Json(report); 
}

  [HttpGet]
[Route("Announcements")]
public async Task<IActionResult> Announcements()
{
    var Announcements = await _context.Announcements.ToListAsync();
    return Json(Announcements); 
}
  [HttpGet]
[Route("Investors-Contact")]
public async Task<IActionResult> InvestorsContact()
{
    var InvestorsContact = await _context.Investor.ToListAsync();
    return Json(InvestorsContact); 
}

  [HttpGet]
[Route("Public-Disclosure")]
public async Task<IActionResult> PublicDisclosure()
{
    var PublicDisclosure = await _context.Disclosure.ToListAsync();
    return Json(PublicDisclosure); 
}

  [HttpGet]
[Route("Forms-TDS")]
public async Task<IActionResult> FormsTDS()
{
    var Formdts = await _context.Formdts.ToListAsync();
    return Json(Formdts); 
}

  [HttpGet]
[Route("appointment-order")]
public async Task<IActionResult> Apporder()
{
    var Apporder = await _context.Apporder.ToListAsync();
    return Json(Apporder); 
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
