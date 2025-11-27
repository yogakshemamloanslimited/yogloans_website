using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using yogloansdotnet.Data;
using yogloansdotnet.Models;
using static QRCoder.PayloadGenerator;

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
        public async Task<IActionResult> addinvestor(InvestoresGroup model, IFormFile Profile)
        {
            try
            {
                byte[]? imagebyte = null;

                if(Profile != null && Profile.Length > 0)
                {
                    using var sm = new MemoryStream();
                    await Profile.CopyToAsync(sm);
                    imagebyte = sm.ToArray();
                }
                if(model.Id == 0)
                {
                    var Investor = new InvestoresGroup()
                    {
                        FullName = model.FullName,
                        Role = model.Role,
                        Phone = model.Phone,
                        Mobile = model.Mobile,
                        Address = model.Address,
                        email = model.email,
                        Profile = imagebyte
                    };
                    _context.Investor.Add(Investor);
                }
                else
                {
                    var existing = await _context.Investor.FirstOrDefaultAsync(a => a.Id == model.Id);


                    if (existing != null)
                    {
                        existing.FullName = model.FullName;
                        existing.Role = model.Role;
                        existing.Phone = model.Phone;
                        existing.Mobile = model.Mobile;
                        existing.Address = model.Address;
                        existing.email = model.email;
                        if (imagebyte != null)
                        {
                            existing.Profile = imagebyte;
                        }

                    }
                    
                    _context.Investor.Update(existing);
                }
               await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Contact Saved Successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving investor");
                return Json(new { success = false, message = "An error occurred while saving the investor. Please try again." });
            }
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

    
      [Route("welcome")]
   public IActionResult welcomes() {
    var data = _context.InvestorsWelcome.ToList(); 
    return View("Views/Admin/Investors/welcome.cshtml", data);
  }

         [Route("add-welcome")]
        [HttpPost]
        public async Task<IActionResult> Addwelcome(
           InvestorsWelcome model,
            IFormFile Image1,
            IFormFile Image2
          )
        {
            try
            {
                byte[]? imagebytes1 = null;
                byte[]? imagebyte2 = null;
                if(Image1 != null && Image1.Length > 0)
                {
                    using var em = new MemoryStream();
                    await Image1.CopyToAsync(em);
                    imagebytes1 = em.ToArray();
                }
                if(Image2 != null && Image2.Length > 0)
                {
                    using var em = new MemoryStream();
                    await Image2.CopyToAsync(em);
                    imagebyte2 = em.ToArray();
                }
                var existing = await _context.InvestorsWelcome.FirstOrDefaultAsync();

                if (existing == null)
                {
                    var welcome = new InvestorsWelcome
                    {
                        Image1 = imagebytes1,
                        Image2 = imagebyte2,
                        Mainhead = model.Mainhead,
                        Subhead =  model.Subhead

                    };
                    _context.InvestorsWelcome.Add(welcome);
                }
                
                else
                {
                    existing.Mainhead = model.Mainhead;
                    existing.Subhead = model.Subhead;

                    if(existing.Image1 != null)
                    {
                        existing.Image1 = imagebytes1;
                    }
                    if (existing.Image2 != null)
                    {
                        existing.Image2 = imagebyte2;
                    }
                    _context.InvestorsWelcome.Update(existing);
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = $" Welcome details added successfully!";
                return RedirectToAction("welcomes");


            }
            catch(Exception ex)
            {

                _logger.LogError(ex, "Error processing loan create");
                TempData["Error"] = "An error occurred while processing your request.";
                return RedirectToAction("welcomes");
            }
          
        }

      
        

}



}
