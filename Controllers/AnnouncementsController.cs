using iText.Forms.Form.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using yogloansdotnet.Data;
using yogloansdotnet.Models;

namespace yogloansdotnet.Controllers
{
    [Authorize]
    [Route("admin/[controller]")]
    public class AnnouncementsController : Controller
    {
       

        private readonly ILogger<AnnouncementsController> _logger;
        private readonly ApplicationDbContext _context;
        public AnnouncementsController(ILogger<AnnouncementsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("addannouncements")]
        public async Task<IActionResult> Addannouncements(AnnouncementsModel model, IFormFile FilePath)
        {
            try
            {
                byte[]? file = null;

                if (FilePath != null && FilePath.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await FilePath.CopyToAsync(ms);
                    file = ms.ToArray();
                }

                var maxId = await _context.Announcements
                    .MaxAsync(x => (int?)x.Id) ?? 0;

                if (model.Id == 0)
                {
                    var welcome = new AnnouncementsModel
                    {
                        Id = maxId + 1,
                        FilePath = file,
                        Title = model.Title
                    };

                    _context.Announcements.Add(welcome);
                }
                else
                {
                    var existing = await _context.Announcements
                        .FirstOrDefaultAsync(a => a.Id == model.Id);

                    if (existing != null)
                    {
                        existing.Title = model.Title;

                        if (file != null)   // ✅ Correct condition
                            existing.FilePath = file;

                        _context.Announcements.Update(existing);
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Saved successfully!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving announcement");

                return Json(new
                {
                    success = false,
                    message = "Error while saving announcement"
                });
            }
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
