using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ILogger<ServicesController> _logger;
        private readonly ApplicationDbContext _context;

        public ServicesController(ILogger<ServicesController> logger, ApplicationDbContext context)
        {
         _logger = logger;
         _context = context;
        }
[HttpGet]
[Route("services/{loanname}/{id}")]
public async Task<IActionResult> Index(string loanname , string id)
{
    var viewModel = new ServicesLoanModel();

    // Filter all models by loanname
    viewModel.AboutContent = await _context.AboutContent.ToListAsync();

    viewModel.Homwelcome = await _context.Homwelcome
        .Where(x => x.Header == loanname)
        .ToListAsync();

    viewModel.Loanpoint = await _context.LoanPoints
        .Where(lp => lp.Loan == id)
        .ToListAsync();

    viewModel.Loan = await _context.Loans
        .Where(l => l.Loanname == loanname)
        .ToListAsync();

        viewModel.Offer = await _context.Offer
        .Where(o => o.Loan == id)
        .ToListAsync();

    return View(viewModel);
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
