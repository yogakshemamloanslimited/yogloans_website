using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Data;

namespace yogloansdotnet.Controllers
{

    public class ShowannounController : Controller
    {
        private readonly ILogger<ShowannounController> _logger;
        private readonly ApplicationDbContext _context;

        public ShowannounController(
            ILogger<ShowannounController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.AnnouncementsWelcome.ToList();
            return View(data);
        }
    }
}
