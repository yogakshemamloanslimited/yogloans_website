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
    }
}
