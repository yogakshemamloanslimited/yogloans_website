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
    public class AnnualreportController : Controller
    {
        private readonly ILogger<AnnualreportController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AnnualreportController(ILogger<AnnualreportController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("addannualreport")]
        [HttpPost]
        public async Task<IActionResult> AddAnnualReport([FromForm] string Title, IFormFile Pdf, [FromForm] string id)
        {
            if (Pdf == null || Pdf.Length == 0)
            {
                TempData["Error"] = "Please select a PDF file";
                return RedirectToAction("Index", "Investors", new { area = "admin" });
            }

            var allowedTypes = new[] { "application/pdf" };
            if (!allowedTypes.Contains(Pdf.ContentType.ToLower()))
            {
                TempData["Error"] = "Only PDF files are allowed";
                return RedirectToAction("Index", "Investors", new { area = "admin" });
            }

            try
            {
                string filePath = await SavePdfAsync(Pdf);

                if (string.IsNullOrEmpty(id))
                {
                    // Create new report
                    var annualReport = new AnnualReportEntity
                    {
                        Title = Title,
                        FilePath = filePath
                    };

                    _context.AnnualReports.Add(annualReport);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Annual report added successfully";
                }
                else
                {
                    // Update existing report
                    var report = await _context.AnnualReports.FindAsync(int.Parse(id));
                    if (report == null)
                    {
                        TempData["Error"] = "Report not found";
                        return RedirectToAction("Index", "Investors", new { area = "admin" });
                    }

                    // Delete old file if it exists
                    if (!string.IsNullOrEmpty(report.FilePath))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, report.FilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    report.Title = Title;
                    report.FilePath = filePath;
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Annual report updated successfully";
                }

                return Redirect("/admin/AdminInvestor");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving annual report");
                TempData["Error"] = "An error occurred while saving the report. Please try again.";
                return Redirect("/admin/AdminInvestor");
            }
        }

        private async Task<string> SavePdfAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/reports");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/reports/" + fileName;
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var report = await _context.AnnualReports.FindAsync(id);
                if (report == null)
                {
                    return Json(new { success = false, message = "Report not found" });
                }

                if (!string.IsNullOrEmpty(report.FilePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, report.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.AnnualReports.Remove(report);
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
