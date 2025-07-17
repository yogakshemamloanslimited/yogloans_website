using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace yogloansdotnet.Controllers
{
    [Authorize]
[Route("admin/[controller]")]
public class DepartmentController : Controller
{
    private readonly ILogger<DepartmentController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public DepartmentController(ILogger<DepartmentController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [Route("add-career")]
    [HttpPost]
    public async Task<IActionResult> AddCareer([FromForm] CareerModel careerModel, [FromForm] IFormFile File)
    {
        try
        {
            // Clear Id validation errors for new records
            if (careerModel.Id == 0)
            {
                ModelState.Remove("Id");
            }

          /*   if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                return Json(new
                {
                    success = false,
                    message = "Invalid data.",
                    modelErrors = errors
                });
            } */

            string savedFilePath = string.Empty;

            if (File != null && File.Length > 0)
            {
                savedFilePath = await SavePdfAsync(File); // Save and get path
                careerModel.File = savedFilePath;
            }

            if (careerModel.Id == 0) // New record
            {   careerModel.Status = "1"; 
                _context.Career.Add(careerModel);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Career added successfully",
                    Id = careerModel.Id
                });
            }
            else // Update existing record
            {
                var existingCareer = await _context.Career.FindAsync(careerModel.Id);
                if (existingCareer == null)
                    return Json(new { success = false, message = "Career not found" });

                existingCareer.Job = careerModel.Job;
                existingCareer.Role = careerModel.Role;
                existingCareer.Department = careerModel.Department;
                existingCareer.Salary_range_from = careerModel.Salary_range_from;
                existingCareer.Salary_range_to = careerModel.Salary_range_to;
                existingCareer.Shift = careerModel.Shift;
                existingCareer.Discription = careerModel.Discription;
                existingCareer.de_name = careerModel.de_name;
                existingCareer.Experience_from = careerModel.Experience_from ;
                existingCareer.Experience_to = careerModel.Experience_to ;
                existingCareer.Status = "1";

                if (!string.IsNullOrEmpty(savedFilePath))
                {
                    // Optional: delete old file if necessary
                    if (!string.IsNullOrEmpty(existingCareer.File))
                    {
                        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, existingCareer.File.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                    existingCareer.File = savedFilePath;
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Career updated successfully",
                    id = existingCareer.Id
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving career");
            return Json(new { success = false, message = ex.ToString() }); // For debugging only!
        }
    }
    [Route("status-change")]
    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int id)
    {
        var career = await _context.Career.FindAsync(id);
        if (career == null)
            return Json(new { success = false, message = "Career not found" });

        // Toggle status
        career.Status = (career.Status == "1") ? "0" : "1";
        _context.Career.Update(career);
        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            message = "Status updated successfully",
            newStatus = career.Status
        });
    }
    private async Task<string> SavePdfAsync(IFormFile file)
    {
        var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/career/doc");

        if (!Directory.Exists(uploads))
            Directory.CreateDirectory(uploads);

        var filePath = Path.Combine(uploads, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return "/uploads/career/doc/" + fileName; // ✅ Corrected path
    }

    [HttpPost("delete-career/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCareer(int id)
    {
        try
        {
            var career = await _context.Career.FindAsync(id);
            if (career == null)
                return Json(new { success = false, message = "Career not found" });

            // Delete file if exists
            if (!string.IsNullOrEmpty(career.File))
            {
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, career.File.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            _context.Career.Remove(career);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Career deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting career");
            return Json(new { success = false, message = ex.Message });
        }
    }
}

}