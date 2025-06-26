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
    }
}