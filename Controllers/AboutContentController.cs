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
    public class AboutContentController : Controller
    {
        private readonly ILogger<AboutContentController> _logger;
        private readonly ApplicationDbContext _context;

        public AboutContentController(ILogger<AboutContentController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
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
    }
}
