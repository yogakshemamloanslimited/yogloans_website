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
    public class UnpaidController : Controller
    {
        private readonly ILogger<UnpaidController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UnpaidController(ILogger<UnpaidController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("add-Unpaid")]
        [HttpPost]
        public async Task<IActionResult> addUnpaid([FromForm] string Title, IFormFile Pdf, [FromForm] string ids)
        {
            try
            {
                string filePath = await SavePdfAsync(Pdf);

                if (string.IsNullOrEmpty(ids))
                {
                    // Create new disclosure
                    var Unpaid = new UnpaidModel
                    {
                        Title = Title,
                        FilePath = filePath
                    };

                    _context.Unpaid.Add(Unpaid);
                    await _context.SaveChangesAsync();

                    return Json(new { 
                        success = true, 
                        message = "Disclosure added successfully",
                        id = Unpaid.Id,
                        title = Unpaid.Title,
                        filePath = Unpaid.FilePath
                    });
                }
                else
                {
                    // Update existing disclosure
                    var existingDisclosure = await _context.Unpaid.FindAsync(int.Parse(ids));
                    if (existingDisclosure == null)
                    {
                        return Json(new { success = false, message = "Disclosure not found" });
                    }

                    // Delete old file if it exists
                    if (!string.IsNullOrEmpty(existingDisclosure.FilePath))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingDisclosure.FilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    
                    existingDisclosure.Title = Title;
                    existingDisclosure.FilePath = filePath;
                  
                    await _context.SaveChangesAsync();

                    return Json(new { 
                        success = true, 
                        message = "Disclosure updated successfully",
                        id = existingDisclosure.Id,
                        title = existingDisclosure.Title,
                        filePath = existingDisclosure.FilePath
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving disclosure");
                return Json(new { success = false, message = "An error occurred while saving the disclosure. Please try again." });
            }
        }

        private async Task<string> SavePdfAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Unpaid");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/Unpaid/" + fileName;
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Unpaid = await _context.Unpaid.FindAsync(id);
                if (Unpaid == null)
                {
                    return Json(new { success = false, message = "Unpaid not found" });
                }

                if (!string.IsNullOrEmpty(Unpaid.FilePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, Unpaid.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Unpaid.Remove(Unpaid);
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
