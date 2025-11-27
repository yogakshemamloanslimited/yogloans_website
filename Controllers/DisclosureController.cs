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
    public class DisclosureController : Controller
    {
        private readonly ILogger<DisclosureController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DisclosureController(ILogger<DisclosureController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("add-disclosure")]
        [HttpPost]
        public async Task<IActionResult> adddisclosure(DisclosureModel model, IFormFile FilePath)
        {
            try
            {
             
                byte[] imageBytes = null;

                // Convert uploaded file to byte array
                if (FilePath != null && FilePath.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await FilePath.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                if (model.Id == 0)
                {
                    
                    var newAnnouncement = new DisclosureModel
                    {
                        Title = model.Title,
                    
                        FilePath = imageBytes
                    };

                    _context.Disclosure.Add(newAnnouncement);
                }
                else
                {
                    // Update existing announcement
                    var existing = await _context.Disclosure
                        .FirstOrDefaultAsync(a => a.Id == model.Id);

                    if (existing != null)
                    {
                        existing.Title = model.Title;
                       

                        // Only update image if a new file is uploaded
                        if (imageBytes != null)
                        {
                            existing.FilePath = imageBytes;
                        }

                        _context.Disclosure.Update(existing);
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
                var disclosure = await _context.Disclosure.FindAsync(id);
                if (disclosure == null)
                {
                    return Json(new { success = false, message = "Disclosure not found" });
                }

                
                _context.Disclosure.Remove(disclosure);
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
