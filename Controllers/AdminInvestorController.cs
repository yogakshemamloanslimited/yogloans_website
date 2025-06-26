using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    [Authorize] // 🔐 This makes the entire controller secure
    [Route("admin/[controller]")]
    public class AdminInvestorController : Controller
    {
        private readonly ILogger<AdminInvestorController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminInvestorController(ILogger<AdminInvestorController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var viewModel = new InvestorViewModel
            {
                Report = await _context.AnnualReports.ToListAsync(),
                Announcements = await _context.Announcements.ToListAsync(),
                Investor = await _context.Investor.ToListAsync(),
                Disclosure = await _context.Disclosure.ToListAsync(),
                Formdts = await _context.Formdts.ToListAsync(),
                Unpaid = await _context.Unpaid.ToListAsync(),
                Apporder = await _context.Apporder.ToListAsync()
            };
            return View("~/Views/admin/Investors/index.cshtml", viewModel);
        }
    }
}