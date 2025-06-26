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
    public class PolicyadminController : Controller
    {
        private readonly ILogger<PolicyadminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PolicyadminController(ILogger<PolicyadminController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            
              var Policy = await _context.Policy.ToListAsync();
               
            return View("~/Views/admin/policy/index.cshtml", Policy);
        }
    

      [Route("add-policy")]
        [HttpPost]
        public async Task<IActionResult> addpolicy([FromForm] string Title, IFormFile Pdf, [FromForm] string id)
        {
            try
            {
                string filePath = await SavePdfAsync(Pdf);

                if (string.IsNullOrEmpty(id))
                {
                    // Create new disclosure
                    var Policy = new PolicyModel
                    {
                        Title = Title,
                        FilePath = filePath
                    };

                    _context.Policy.Add(Policy);
                    await _context.SaveChangesAsync();

                    return Json(new { 
                        success = true, 
                        message = "Policy added successfully",
                        id = Policy.Id,
                        title = Policy.Title,
                        filePath = Policy.FilePath
                    });
                }
                else
                {
                    // Update existing disclosure
                    var existingDisclosure = await _context.Policy.FindAsync(int.Parse(id));
                    if (existingDisclosure == null)
                    {
                        return Json(new { success = false, message = "Policy not found" });
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
                        message = "CSR updated successfully",
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
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Policy");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/Policy/" + fileName;
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Policy = await _context.Policy.FindAsync(id);
                if (Policy == null)
                {
                    return Json(new { success = false, message = "Policy not found" });
                }

                if (!string.IsNullOrEmpty(Policy.FilePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, Policy.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Policy.Remove(Policy);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Policy");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}