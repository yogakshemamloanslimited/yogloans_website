using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using yogloansdotnet.Data;
using yogloansdotnet.Models;

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
        public async Task<IActionResult> AddAnnualReport(AnnualReportEntity model, IFormFile pdf)
        {
            try
            {
                byte[]? file = null;

                if(pdf != null && pdf.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await pdf.CopyToAsync(ms);
                    file = ms.ToArray();
                }

                if(model.Id == 0)
                {
                    var welcome = new AnnualReportEntity
                    {
                        FilePath = file,
                        Title = model.Title
                    };
                    
                    _context.AnnualReports.Add(welcome);
                }
                else
                {
                    var existing = await _context.AnnualReports.FirstOrDefaultAsync(a => a.Id == model.Id);
                    if(existing != null)
                    {
                        existing.Title = model.Title;
                       
                    }
                    if(existing.FilePath != null)
                    {
                        existing.FilePath = file;
                    }

                    _context.AnnualReports.Update(existing);
                }
                    await  _context.SaveChangesAsync();
                    return Redirect("/admin/AdminInvestor");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving annual report");
                TempData["Error"] = "An error occurred while saving the report. Please try again.";
                return Redirect("/admin/AdminInvestor");
            }
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
