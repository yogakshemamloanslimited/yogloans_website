
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class sbionlinepaymentsuccess : Controller
    {
        private readonly ILogger<sbionlinepaymentsuccess> _logger;
        private readonly ApplicationDbContext _context;

        public sbionlinepaymentsuccess(ILogger<sbionlinepaymentsuccess> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var online = await _context.OnlineWelcome.ToListAsync();
            return View(online);
        }
    }
}
