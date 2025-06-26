using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace yogloansdotnet.Controllers
{
    [Authorize]
    [Route("admin/[controller]")]
    public class AnnouncementsController : Controller
    {
        private readonly ILogger<AnnouncementsController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AnnouncementsController(ILogger<AnnouncementsController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("addannouncements")]
        [HttpPost]
        public async Task<IActionResult> Announcements([FromForm] string Title, IFormFile Pdf, [FromForm] string ids)
        {
            if (Pdf == null || Pdf.Length == 0)
            {
                return Json(new { success = false, message = "Please select a PDF file" });
            }

            var allowedTypes = new[] { "application/pdf" };
            if (!allowedTypes.Contains(Pdf.ContentType.ToLower()))
            {
                return Json(new { success = false, message = "Only PDF files are allowed" });
            }

            try
            {
                string filePath = await SavePdfAsync(Pdf);

                if (string.IsNullOrEmpty(ids))
                {
                    // Create new report
                    var Announcements = new AnnouncementsModel
                    {
                        Title = Title,
                        FilePath = filePath
                    };

                    _context.Announcements.Add(Announcements);
                    await _context.SaveChangesAsync();

                    return Json(new { 
                        success = true, 
                        message = "Announcement added successfully",
                        id = Announcements.Id,
                        title = Announcements.Title,
                        filePath = Announcements.FilePath
                    });
                }
                else
                {
                    // Update existing report
                    var report = await _context.Announcements.FindAsync(int.Parse(ids));
                    if (report == null)
                    {
                        return Json(new { success = false, message = "Report not found" });
                    }

                    // Delete old file if it exists
                    if (!string.IsNullOrEmpty(report.FilePath))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, report.FilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    report.Title = Title;
                    report.FilePath = filePath;
                    await _context.SaveChangesAsync();

                    return Json(new { 
                        success = true, 
                        message = "Announcement updated successfully",
                        id = report.Id,
                        title = report.Title,
                        filePath = report.FilePath
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving announcement");
                return Json(new { success = false, message = "An error occurred while saving the announcement. Please try again." });
            }
        }

        private async Task<string> SavePdfAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Announcements");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/Announcements/" + fileName;
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var report = await _context.Announcements.FindAsync(id);
                if (report == null)
                {
                    return Json(new { success = false, message = "Report not found" });
                }

                if (!string.IsNullOrEmpty(report.FilePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, report.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Announcements.Remove(report);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting report");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
