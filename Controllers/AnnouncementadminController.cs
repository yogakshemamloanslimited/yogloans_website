using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using yogloansdotnet.Data;
using yogloansdotnet.Models;

namespace yogloansdotnet.Controllers
{
    [Authorize]
    [Route("admin/[controller]")]
    public class AnnouncementadminController : Controller
    {
        private readonly ILogger<AnnouncementadminController> _logger;
        private readonly ApplicationDbContext _context;

        public AnnouncementadminController(
            ILogger<AnnouncementadminController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: admin/Announcementadmin/welcome
        [HttpGet("welcome")]
        public IActionResult Welcome()
        {
            var data = _context.AnnouncementsWelcome.ToList();
            return View("~/Views/admin/Announcementadmin/welcome.cshtml", data);
        }


        // Detect PDF file


        // POST: admin/Announcementadmin/welcome-create
        [HttpPost("welcome-create")]
        public async Task<IActionResult> WelcomeCreate(
      AnnouncementsWelcomeModel model,
      IFormFile ImageFile,
      IFormFile ImageFile2)
        {
            try
            {
                byte[] imageBytes1 = null;
                byte[] imageBytes2 = null;

                // First image
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    using var ms1 = new MemoryStream();
                    await ImageFile.CopyToAsync(ms1);
                    imageBytes1 = ms1.ToArray();
                }

                // Second image
                if (ImageFile2 != null && ImageFile2.Length > 0)
                {
                    using var ms2 = new MemoryStream();
                    await ImageFile2.CopyToAsync(ms2);
                    imageBytes2 = ms2.ToArray();
                }

                // Get existing record
                var existing = await _context.AnnouncementsWelcome.FirstOrDefaultAsync();

                if (existing != null)
                {
                    existing.title = model.title;

                    if (imageBytes1 != null)
                        existing.image = imageBytes1;

                    if (imageBytes2 != null)
                        existing.image2 = imageBytes2;

                    _context.AnnouncementsWelcome.Update(existing);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Content updated successfully!";
                }
                else
                {
                    var obj = new AnnouncementsWelcomeModel
                    {
                        title = model.title,
                        image = imageBytes1,
                        image2 = imageBytes2
                    };

                    _context.AnnouncementsWelcome.Add(obj);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Content created successfully!";
                }

                return RedirectToAction("Welcome");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving content");
                TempData["Error"] = "An error occurred. Try again.";
                return RedirectToAction("Welcome");
            }
        }
        [HttpGet("announcements")]
        public IActionResult announcements()
        {
            var data = _context.announcements_create.ToList();
            return View("~/Views/admin/Announcementadmin/announcements.cshtml", data);
        }
        [HttpPost("add-aanouncements")]
        public async Task<IActionResult> add(
        announcements_create model,
        IFormFile ImageFile
    )
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
                    var newAnnouncement = new announcements_create
                    {
                        title = model.title,
                        content = model.content,
                        image = imageBytes
                    };

                    _context.announcements_create.Add(newAnnouncement);
                }
                else
                {
                    // Update existing announcement
                    var existing = await _context.announcements_create
                        .FirstOrDefaultAsync(a => a.Id == model.Id);

                    if (existing != null)
                    {
                        existing.title = model.title;
                        existing.content = model.content;

                        // Only update image if a new file is uploaded
                        if (imageBytes != null)
                        {
                            existing.image = imageBytes;
                        }

                        _context.announcements_create.Update(existing);
                    }
                    else
                    {
                        TempData["Error"] = "Announcement not found!";
                        return RedirectToAction("announcements");
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Announcement saved successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving content");
                TempData["Error"] = "An error occurred. Try again.";
            }

            return RedirectToAction("announcements");
        }

        [HttpPost("DeleteAnnoun")]
        public async Task<IActionResult> DeleteAnnoun(int id)
        {
            try
            {
                var deletingData = await _context.announcements_create.FindAsync(id);
                if (deletingData == null)
                    return NotFound("Announcement not found.");

                _context.announcements_create.Remove(deletingData);
                await _context.SaveChangesAsync();

                return Ok("Announcement deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting announcement");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
