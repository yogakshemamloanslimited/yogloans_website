using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    [Authorize]
    [Route("admin/[controller]")]
    public class InvestorgroupController : Controller
    {
        private readonly ILogger<InvestorgroupController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InvestorgroupController(ILogger<InvestorgroupController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("addinvestor")]
        [HttpPost]
        public async Task<IActionResult> addinvestor([FromForm] string Fullname, IFormFile Profile, [FromForm] string ids, [FromForm] string Role, [FromForm] string Phone, [FromForm] string Mobile, [FromForm] string Address)
        {
            try
            {
                string filePath = await SavePdfAsync(Profile);

                if (string.IsNullOrEmpty(ids))
                {
                    // Create new investor
                    var InvestoresGroup = new InvestoresGroup
                    {
                        FullName = Fullname,
                        Phone = Phone,
                        Mobile = Mobile,
                        Address = Address,
                        Profile = filePath,
                        Role = Role
                    };

                    _context.Investor.Add(InvestoresGroup);
                    await _context.SaveChangesAsync();

                    return Json(new { 
                        success = true, 
                        message = "Investor added successfully",
                        id = InvestoresGroup.Id,
                        title = InvestoresGroup.FullName,
                        filePath = InvestoresGroup.Profile
                    });
                }
                else
                {
                    // Update existing investor
                    var existingInvestor = await _context.Investor.FindAsync(int.Parse(ids));
                    if (existingInvestor == null)
                    {
                        return Json(new { success = false, message = "Investor not found" });
                    }

                    // Delete old file if it exists
                    if (!string.IsNullOrEmpty(existingInvestor.Profile))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingInvestor.Profile.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    
                    existingInvestor.FullName = Fullname;
                    existingInvestor.Phone = Phone;
                    existingInvestor.Mobile = Mobile;
                    existingInvestor.Address = Address;
                    existingInvestor.Profile = filePath;
                    existingInvestor.Role = Role;
                    await _context.SaveChangesAsync();

                    return Json(new { 
                        success = true, 
                        message = "Investor updated successfully",
                        id = existingInvestor.Id,
                        title = existingInvestor.FullName,
                        filePath = existingInvestor.Profile
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving investor");
                return Json(new { success = false, message = "An error occurred while saving the investor. Please try again." });
            }
        }

        private async Task<string> SavePdfAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/investor");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/investor/" + fileName;
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var report = await _context.Investor.FindAsync(id);
                if (report == null)
                {
                    return Json(new { success = false, message = "Investor not found" });
                }

                if (!string.IsNullOrEmpty(report.Profile))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, report.Profile.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Investor.Remove(report);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting investor");
                return Json(new { success = false, message = ex.Message });
            }
        }

    
      [Route("welcome")]
   public IActionResult welcomes() {
    var data = _context.InvestorsWelcome.ToList(); 
    return View("Views/Admin/Investors/welcome.cshtml", data);
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
                var welcome = await _context.Set<InvestorsWelcome>().FirstOrDefaultAsync();
                if (welcome == null)
                {
                    // Create new welcome entry
                    welcome = new InvestorsWelcome
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
            var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/investors");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/investors/" + fileName;
        }
        

}



}
