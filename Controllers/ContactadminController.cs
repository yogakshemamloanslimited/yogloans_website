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
    public class ContactadminController : Controller
    {
        private readonly ILogger<ContactadminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactadminController(ILogger<ContactadminController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

      
      [Route("welcome")]
   public IActionResult welcomes() {
    var data = _context.ContactWelcome.ToList(); 
    return View("Views/Admin/contact/welcome.cshtml", data);
  }

         [Route("add-welcome")]
        [HttpPost]
        public async Task<IActionResult> Addwelcome(
          ContactWelcome model,
            IFormFile Image1,
            IFormFile Image2)
        {
            try
            {
                byte[]? imagebytes1 = null;
                byte[]? imagebyte2 = null;
                if (Image1 != null && Image1.Length > 0)
                {
                    using var em = new MemoryStream();
                    await Image1.CopyToAsync(em);
                    imagebytes1 = em.ToArray();
                }
                if (Image2 != null && Image2.Length > 0)
                {
                    using var em = new MemoryStream();
                    await Image2.CopyToAsync(em);
                    imagebyte2 = em.ToArray();
                }
                var existing = await _context.ContactWelcome.FirstOrDefaultAsync();

                if (existing == null)
                {
                    var welcome = new ContactWelcome
                    {
                        Image1 = imagebytes1,
                        Image2 = imagebyte2,
                        Mainhead = model.Mainhead,
                        Subhead = model.Subhead

                    };
                    _context.ContactWelcome.Add(welcome);
                }

                else
                {
                    existing.Mainhead = model.Mainhead;
                    existing.Subhead = model.Subhead;

                    if (imagebytes1 != null)
                    {
                        existing.Image1 = imagebytes1;
                    }
                    if (imagebyte2 != null)
                    {
                        existing.Image2 = imagebyte2;
                    }
                    _context.ContactWelcome.Update(existing);
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = $" Welcome details added successfully!";
                return RedirectToAction("welcomes");


            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error processing loan create");
                TempData["Error"] = "An error occurred while processing your request.";
                return RedirectToAction("welcomes");
            }

        }

        

}

}