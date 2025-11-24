using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using yogloansdotnet.Data;
using yogloansdotnet.Models;

namespace yogloansdotnet.Controllers
{
 
    public class AnnouncementController : Controller
    {
        private readonly ILogger<AnnouncementController> _logger;
        private readonly ApplicationDbContext _context;

        public AnnouncementController(ILogger<AnnouncementController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            var model = new AnnouncementData
            {
                Create = _context.announcements_create.ToList(),
                Welcome = _context.AnnouncementsWelcome.FirstOrDefault()
            };

            return View(model);
        }



        [HttpPost]
        public IActionResult contentsession([FromBody] ContentDto data)
        {
            HttpContext.Session.SetString("content", data.Content);

            return Json(new { success = true });
        }

        public class ContentDto
        {
            public string Content { get; set; }
        }

    }
}
