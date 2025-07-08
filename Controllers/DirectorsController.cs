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
    public class DirectorsController : Controller
    {
        private readonly ILogger<DirectorsController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DirectorsController(ILogger<DirectorsController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
[Route("")]
[Route("index")]
public async Task<IActionResult> Index()
{
    var profiles = await _context.Directors.ToListAsync();
    return View("~/Views/admin/Directors/index.cshtml", profiles);
}


      [Route("add-director")]
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
            var Directors = new DirectorsModel
            {
                Name = Name,
                Profile = filePath ?? "",
                Post = Post,
                About = About
            };

            _context.Directors.Add(Directors);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Directors added successfully" });
        }
        else
        {
            // Update existing director
            var existingDirectors = await _context.Directors.FindAsync(int.Parse(id));
            if (existingDirectors == null)
            {
                return Json(new { success = false, message = "Directors not found" });
            }

            // Delete old file if new file is uploaded
            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(existingDirectors.Profile))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingDirectors.Profile.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                existingDirectors.Profile = filePath;
            }

            existingDirectors.Name = Name;
            existingDirectors.Post = Post;
            existingDirectors.About = About;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Director updated successfully" });
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error saving director");
        return Json(new { success = false, message = "An error occurred while saving the director. Please try again." });
    }
}

        private async Task<string> SavePdfAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Directors");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/Directors/" + fileName;
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Directors = await _context.Directors.FindAsync(id);
                if (Directors == null)
                {
                    return Json(new { success = false, message = "Directors not found" });
                }

                if (!string.IsNullOrEmpty(Directors.Profile))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, Directors.Profile.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Directors.Remove(Directors);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Directors");
                return Json(new { success = false, message = ex.Message });
            }
        }
    
    
    
    


[HttpGet("get-model/{id}")]
public async Task<IActionResult> GetDirector(int id)
{
    try
    {
        var director = await _context.Directors.FindAsync(id);

        if (director == null)
        {
            return Json(new { success = false, message = "Director not found" });
        }

        return Json(new { success = true, data = director });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching Director data");
        return Json(new { success = false, message = ex.Message });
    }
}

    
    
    
    
    }}