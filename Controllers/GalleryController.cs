using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class GalleryController : Controller
    {
        private readonly ILogger<GalleryController> _logger;
        private readonly ApplicationDbContext _context;

        public GalleryController(ILogger<GalleryController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
       public async Task<IActionResult> Index()
{
    var galleries = await _context.Gallery.ToListAsync();
    var galleriesImages = await _context.GalleryImages.ToListAsync();

    var model = new GalleryViewModel
    {
        WelcomeGallery = galleries,
        OtherGallery = galleriesImages
    };

    return View(model);
}


    }
}