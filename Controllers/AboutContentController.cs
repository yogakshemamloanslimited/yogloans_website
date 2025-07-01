using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace yogloansdotnet.Controllers
{
    [Authorize] // 🔐 This makes the entire controller secure
    [Route("admin/[controller]")]
    public class AboutContentController : Controller
    {
        private readonly ILogger<AboutContentController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AboutContentController(ILogger<AboutContentController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("")]
        [Route("About")]
        public async Task<IActionResult> About()
        {
            // This will look for /views/admin/About/index.cshtml
            var content = await _context.AboutContent.FirstOrDefaultAsync();
            return View("/views/admin/About/index.cshtml", content);
        }

        [HttpPost]
        [Route("about-create")]
        public async Task<IActionResult> Aboutcreate(AboutContentModel model)
        {
            try
            {
                _logger.LogInformation($"Received form submission. Header: {model.Header}, Content: {model.Content}");

                if (string.IsNullOrWhiteSpace(model.Header))
                {
                    TempData["Error"] = "Header field is required";
                    return RedirectToAction("About", "AboutContent");
                }

                if (string.IsNullOrWhiteSpace(model.Content))
                {
                    TempData["Error"] = "Content field is required";
                    return RedirectToAction("About", "AboutContent");
                }

                // Check if there's already content in the database
                var existingContent = await _context.AboutContent.FirstOrDefaultAsync();
                if (existingContent != null)
                {
                    // Update existing content
                    existingContent.Header = model.Header;
                    existingContent.Content = model.Content;
                    _context.AboutContent.Update(existingContent);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "About content updated successfully!";
                }
                else
                {
                    // Create new content
                    var aboutContent = new AboutContentModel
                    {
                        Header = model.Header,
                        Content = model.Content
                    };
                    _context.AboutContent.Add(aboutContent);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "About content created successfully!";
                }
                
                return RedirectToAction("About", "AboutContent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving about content");
                TempData["Error"] = "An error occurred while saving content. Please try again.";
                return RedirectToAction("About", "AboutContent");
            }
        }

        [HttpPost]
        [Route("empty-table")]
        public async Task<IActionResult> EmptyTable()
        {
            try
            {
                _context.AboutContent.RemoveRange(_context.AboutContent);
                await _context.SaveChangesAsync();
                TempData["Success"] = "About content table has been emptied successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while emptying about content table");
                TempData["Error"] = "An error occurred while emptying the table. Please try again.";
            }
            return RedirectToAction("About", "AboutContent");
        }
    


    
      [Route("welcome")]
   public IActionResult welcomes() {
    var data = _context.AboutWelcome.ToList(); 
    return View("Views/Admin/About/welcome.cshtml", data);
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
                var welcome = await _context.Set<AboutWelcome>().FirstOrDefaultAsync();
                if (welcome == null)
                {
                    // Create new welcome entry
                    welcome = new AboutWelcome
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
            var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/about");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/about/" + fileName;
        }
        

}

}