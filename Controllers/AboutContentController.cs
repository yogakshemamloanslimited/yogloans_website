using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Identity.Client;

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
     AboutWelcome model,
     IFormFile Image1,
     IFormFile Image2)
        {
            try
            {
                var existing = await _context.AboutWelcome
                                    .FirstOrDefaultAsync(a => a.Id == model.Id);

                byte[]? imagebytes = null;
                byte[]? imagebyte2 = null;

                // ✅ Image 1
                if (Image1 != null && Image1.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await Image1.CopyToAsync(ms);
                    imagebytes = ms.ToArray();
                }

                // ✅ Image 2
                if (Image2 != null && Image2.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await Image2.CopyToAsync(ms);
                    imagebyte2 = ms.ToArray();
                }

                // ✅ INSERT
                if (existing == null)
                {
                    var welcome_create = new AboutWelcome
                    {
                        Image1 = imagebytes,
                        Image2 = imagebyte2,
                        Mainhead = model.Mainhead,
                        Subhead = model.Subhead
                    };

                    _context.AboutWelcome.Add(welcome_create);
                }
                // ✅ UPDATE
                else
                {
                    if (imagebytes != null)
                        existing.Image1 = imagebytes;

                    if (imagebyte2 != null)
                        existing.Image2 = imagebyte2;

                    existing.Mainhead = model.Mainhead;
                    existing.Subhead = model.Subhead;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Welcome content saved successfully!";
                return RedirectToAction(nameof(welcomes));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving about welcome content");
                TempData["Error"] = "An error occurred while saving. Please try again.";
                return RedirectToAction(nameof(welcomes));
            }
        }




    }

}