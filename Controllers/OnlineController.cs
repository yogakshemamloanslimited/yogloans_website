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
    public class OnlineController : Controller
    {
        private readonly ILogger<OnlineController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OnlineController(ILogger<OnlineController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();
            return View("~/Views/admin/Online/index.cshtml", services);
        }
    

      [Route("add-services")]
        [HttpPost]
        public async Task<IActionResult> addservices([FromForm] string Title,[FromForm] string Subtitle, IFormFile image, [FromForm] string id)
        {
            try
            {
                string filePath = await SavePdfAsync(image);

                if (string.IsNullOrEmpty(id))
                {
                    // Create new service
                    var Services = new ServicesModel
                    {
                        Title = Title,
                        Subtitle = Subtitle,
                        FilePath = filePath
                    };

                    _context.Services.Add(Services);
                    await _context.SaveChangesAsync();

                    return Json(new { 
                        success = true, 
                        message = "Service added successfully",
                        id = Services.Id,
                        title = Services.Title,
                        subtitle = Services.Subtitle,
                        filePath = Services.FilePath
                    });
                }
                else
                {
                    // Update existing service
                    var existingService = await _context.Services.FindAsync(int.Parse(id));
                    if (existingService == null)
                    {
                        return Json(new { success = false, message = "Service not found" });
                    }

                    // Delete old file if it exists
                    if (!string.IsNullOrEmpty(existingService.FilePath))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingService.FilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    
                    existingService.Title = Title;
                    existingService.Subtitle = Subtitle;
                    existingService.FilePath = filePath;
                  
                    await _context.SaveChangesAsync();

                    return Json(new { 
                        success = true, 
                        message = "Service updated successfully",
                        id = existingService.Id,
                        title = existingService.Title,
                        subtitle = existingService.Subtitle,
                        filePath = existingService.FilePath
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving service");
                return Json(new { success = false, message = "An error occurred while saving the service. Please try again." });
            }
        }

        private async Task<string> SavePdfAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Services");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/Services/" + fileName;
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Services = await _context.Services.FindAsync(id);
                if (Services == null)
                {
                    return Json(new { success = false, message = "Services not found" });
                }

                if (!string.IsNullOrEmpty(Services.FilePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, Services.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Services.Remove(Services);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Services");
                return Json(new { success = false, message = ex.Message });
            }
        }

        
    
      [Route("welcome")]
   public IActionResult welcomes() {
    var data = _context.OnlineWelcome.ToList(); 
    return View("Views/Admin/Online/welcome.cshtml", data);
  }

         [Route("add-welcome")]
        [HttpPost]
        public async Task<IActionResult> Addwelcome(
            [FromForm] string Mainhead,
            [FromForm] string Subhead,
            IFormFile Image1,
            IFormFile Image2,
            [FromForm] string ExistingImage1,
            [FromForm] string ExistingImage2)
        {
            // Only require an image if there is no existing image and no new file
            if ((Image1 == null || Image1.Length == 0) && string.IsNullOrEmpty(ExistingImage1))
            {
                TempData["Error"] = "Please select the desktop image.";
                return RedirectToAction("welcomes");
            }
            if ((Image2 == null || Image2.Length == 0) && string.IsNullOrEmpty(ExistingImage2))
            {
                TempData["Error"] = "Please select the mobile image.";
                return RedirectToAction("welcomes");
            }

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (Image1 != null && Image1.Length > 0 && !allowedTypes.Contains(Image1.ContentType.ToLower()))
            {
                TempData["Error"] = "Only image files are allowed for desktop image.";
                return RedirectToAction("welcomes");
            }
            if (Image2 != null && Image2.Length > 0 && !allowedTypes.Contains(Image2.ContentType.ToLower()))
            {
                TempData["Error"] = "Only image files are allowed for mobile image.";
                return RedirectToAction("welcomes");
            }

            try
            {
                string image1Path = ExistingImage1;
                string image2Path = ExistingImage2;

                if (Image1 != null && Image1.Length > 0)
                {
                    image1Path = await SaveImageAsync(Image1);
                    // Optionally delete old image1 file here if you want
                }
                if (Image2 != null && Image2.Length > 0)
                {
                    image2Path = await SaveImageAsync(Image2);
                    // Optionally delete old image2 file here if you want
                }

                // Only one record should exist
                var welcome = await _context.Set<OnlineWelcome>().FirstOrDefaultAsync();
                if (welcome == null)
                {
                    // Create new welcome entry
                    welcome = new OnlineWelcome
                    {
                        Mainhead = Mainhead,
                        Subhead = Subhead,
                        Image1 = image1Path,
                        Image2 = image2Path
                    };
                    _context.Add(welcome);
                    TempData["Success"] = "Welcome section added successfully.";
                }
                else
                {
                    // Optionally delete old images if you replaced them
                    if ((Image1 != null && Image1.Length > 0) && !string.IsNullOrEmpty(welcome.Image1))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, welcome.Image1.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    if ((Image2 != null && Image2.Length > 0) && !string.IsNullOrEmpty(welcome.Image2))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, welcome.Image2.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    welcome.Mainhead = Mainhead;
                    welcome.Subhead = Subhead;
                    welcome.Image1 = image1Path;
                    welcome.Image2 = image2Path;
                    TempData["Success"] = "Welcome section updated successfully.";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("welcomes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving welcome section");
                TempData["Error"] = "An error occurred while saving. Please try again.";
                return RedirectToAction("welcomes");
            }
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/online");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/online/" + fileName;
        }
        

}
    }
