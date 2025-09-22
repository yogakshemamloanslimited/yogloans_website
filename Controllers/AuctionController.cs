using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class AuctionController : Controller
    {
        public IActionResult Index()
        {    var vm = new LoanGroupViewModel
    {
        Gold = _context.Homwelcome.Where(x => x.LoanType == "Gold").ToList(),
        Business = _context.Homwelcome.Where(x => x.LoanType == "Business").ToList(),
        Vehicle = _context.Homwelcome.Where(x => x.LoanType == "Vehicle").ToList(),
        CD = _context.Homwelcome.Where(x => x.LoanType == "CD").ToList(),
        AboutContent = _context.AboutContent.ToList()
    };
            return View(vm);
        }
private readonly ILogger<AuctionController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
            public AuctionController(ILogger<AuctionController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
public IActionResult Welcome()
{
   

    return View("~/Views/Auction/welcome.cshtml");
}


    }
}
