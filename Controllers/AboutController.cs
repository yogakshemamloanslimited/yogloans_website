using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class AboutController : Controller
    {
        private readonly ILogger<AboutController> _logger;
        private readonly ApplicationDbContext _context;

        public AboutController(ILogger<AboutController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
public async Task<IActionResult> Index()
{
    var About = await _context.AboutWelcome.ToListAsync();
    var Aboutcontent = await _context.AboutContent.ToListAsync();
    var Directors = await _context.Directors.ToListAsync();
    var Leaders = await _context.Leaders.ToListAsync();
    var viewModel = new AboutViewModel
    {
        About = About,
        AboutContent = Aboutcontent,
        Directors = Directors,
        Leaders = Leaders
    };

    return View(viewModel);
}

[HttpGet("get-model/{id}")]
public async Task<IActionResult> GetDirector(int id)
{
    try
    {
        var director = await _context.Directors.FindAsync(id);

        if (director == null)
        {
            return Json(new { success = false, message = "Director not found" });
        }

        return Json(new { success = true, data = director });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching Director data");
        return Json(new { success = false, message = ex.Message });
    }
}

    
[HttpGet("get-model-leaders/{id}")]
public async Task<IActionResult> GetLeaders(int id)
{
    try
    {
        var Leaders = await _context.Leaders.FindAsync(id);

        if (Leaders == null)
        {
            return Json(new { success = false, message = "Leaders not found" });
        }

        return Json(new { success = true, data = Leaders });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching Leaders data");
        return Json(new { success = false, message = ex.Message });
    }
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
