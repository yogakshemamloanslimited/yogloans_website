using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using yogloansdotnet.Data;
using yogloansdotnet.Models;

namespace yogloansdotnet.Controllers
{
    [Authorize]
    [Route("admin/[controller]")]
    public class ApporderController : Controller
    {
        private readonly ILogger<ApporderController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ApporderController(ILogger<ApporderController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("add-apporder")]
        [HttpPost]
        public async Task<IActionResult> addapporder(ApporderModel model, IFormFile ImageFile)
        {
            try
            {
                byte[] imageBytes = null;

                // Convert uploaded file to byte array
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await ImageFile.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                if (model.Id == 0)
                {
                    // Create new announcement
                    var newAnnouncement = new ApporderModel
                    {
                        Title = model.Title,

                        FilePath = imageBytes
                    };

                    _context.Apporder.Add(newAnnouncement);
                }
                else
                {
                    // Update existing announcement
                    var existing = await _context.Apporder
                        .FirstOrDefaultAsync(a => a.Id == model.Id);

                    if (existing != null)
                    {
                        existing.Title = model.Title;


                        // Only update image if a new file is uploaded
                        if (imageBytes != null)
                        {
                            existing.FilePath = imageBytes;
                        }

                        _context.Apporder.Update(existing);
                    }
                    else
                    {
                        TempData["Error"] = "Announcement not found!";
                        return RedirectToAction("announcements");
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Saved Successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Apporder");
                return Json(new { success = false, message = "An error occurred while saving the disclosure. Please try again." });
            }
        }

        private async Task<string> SavePdfAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Apporder");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/Apporder/" + fileName;
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Apporder = await _context.Apporder.FindAsync(id);
                if (Apporder == null)
                {
                    return Json(new { success = false, message = "Apporder not found" });
                }

               

                _context.Apporder.Remove(Apporder);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Apporder");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
