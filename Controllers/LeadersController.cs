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
    public class LeadersController : Controller
    {
        private readonly ILogger<LeadersController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LeadersController(ILogger<LeadersController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
[Route("")]
[Route("index")]
public async Task<IActionResult> Index()
{
    var profiles = await _context.Leaders.ToListAsync();
    return View("~/Views/admin/Leaders/index.cshtml", profiles);
}


      [Route("add-leaders")]
[HttpPost]
public async Task<IActionResult> addcsr(
    [FromForm] string Name,
    [FromForm] string Post,
    [FromForm] string About,
    IFormFile Profile,
    [FromForm] string id)
{
    try
    {
        string filePath = null;
        if (Profile != null)
        {
            filePath = await SavePdfAsync(Profile);
        }

        if (string.IsNullOrEmpty(id))
        {
            // Create new director
            var Leaders = new LeadersModel
            {
                Name = Name,
                Profile = filePath ?? "",
                Post = Post,
                About = About
            };

            _context.Leaders.Add(Leaders);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Leaders added successfully" });
        }
        else
        {
            // Update existing director
            var existingLeaders = await _context.Leaders.FindAsync(int.Parse(id));
            if (existingLeaders == null)
            {
                return Json(new { success = false, message = "Leaders not found" });
            }

            // Delete old file if new file is uploaded
            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(existingLeaders.Profile))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingLeaders.Profile.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                existingLeaders.Profile = filePath;
            }

            existingLeaders.Name = Name;
            existingLeaders.Post = Post;
            existingLeaders.About = About;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Leaders updated successfully" });
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error saving Leaders");
        return Json(new { success = false, message = "An error occurred while saving the Leaders. Please try again." });
    }
}

        private async Task<string> SavePdfAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Leaders");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/Leaders/" + fileName;
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Leaders = await _context.Leaders.FindAsync(id);
                if (Leaders == null)
                {
                    return Json(new { success = false, message = "Leaders not found" });
                }

                if (!string.IsNullOrEmpty(Leaders.Profile))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, Leaders.Profile.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Leaders.Remove(Leaders);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Leaders");
                return Json(new { success = false, message = ex.Message });
            }
        }
    
    
    
    


[HttpGet("get-model/{id}")]
public async Task<IActionResult> GetDirector(int id)
{
    try
    {
        var Leaders = await _context.Leaders.FindAsync(id);

        if (Leaders == null)
        {
            return Json(new { success = false, message = "Leaders not found" });
        }

        return Json(new { success = true, data = Leaders });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching Leaders data");
        return Json(new { success = false, message = ex.Message });
    }
}

    
    
    
    
    }}