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
    public class FormdtsController : Controller
    {
        private readonly ILogger<FormdtsController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FormdtsController(ILogger<FormdtsController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("add-addFormdts")]
        [HttpPost]
        public async Task<IActionResult> addFormdts(FormdtsModel model, IFormFile ImageFile)
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
                    var newAnnouncement = new FormdtsModel
                    {
                        Title = model.Title,
                       
                        FilePath = imageBytes
                    };

                    _context.Formdts.Add(newAnnouncement);
                }
                else
                {
                    // Update existing announcement
                    var existing = await _context.Formdts
                        .FirstOrDefaultAsync(a => a.Id == model.Id);

                    if (existing != null)
                    {
                        existing.Title = model.Title;
                       

                        // Only update image if a new file is uploaded
                        if (imageBytes != null)
                        {
                            existing.FilePath = imageBytes;
                        }

                        _context.Formdts.Update(existing);
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
                _logger.LogError(ex, "Error saving disclosure");
                return Json(new { success = false, message = "An error occurred while saving the disclosure. Please try again." });
            }
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Formdts = await _context.Formdts.FindAsync(id);
                if (Formdts == null)
                {
                    return Json(new { success = false, message = "Formdts not found" });
                }

                

                _context.Formdts.Remove(Formdts);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting disclosure");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
