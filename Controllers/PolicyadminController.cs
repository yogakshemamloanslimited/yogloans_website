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


        
    
      [Route("welcome")]
   public IActionResult welcomes() {
    var data = _context.PolicyWelcome.ToList(); 
    return View("Views/Admin/policy/welcome.cshtml", data);
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
                var welcome = await _context.Set<PolicyWelcome>().FirstOrDefaultAsync();
                if (welcome == null)
                {
                    // Create new welcome entry
                    welcome = new PolicyWelcome
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
            var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/policy");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/policy/" + fileName;
        }
        

}
    }
