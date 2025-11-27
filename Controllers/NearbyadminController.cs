using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    [Authorize]
    [Route("admin/[controller]")]
    public class NearbyadminController : Controller
    {
        private readonly ILogger<NearbyadminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NearbyadminController(ILogger<NearbyadminController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        
       

       
        
       /*   [Route("designation")]
        
        public IActionResult designation(){
            return View("Views/Admin/Career/designation.cshtml");
        } */
      [Route("welcome")]

  public async Task<IActionResult> welcomes() {
    var data = _context.Nearby.ToList(); // Fetch CareerWelcomeModel entries
    return View("Views/Admin/Nearby/welcome.cshtml", data);
}

         [Route("add-nearby-welcome")]
        [HttpPost]
        public async Task<IActionResult> Addwelcome(
           NearbyModel model,
            IFormFile Image1,
            IFormFile Image2
          )
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
                var existing = await _context.Nearby.FirstOrDefaultAsync();

                if (existing == null)
                {
                    var welcome = new NearbyModel
                    {
                        Image1 = imagebytes1,
                        Image2 = imagebyte2,
                        Mainhead = model.Mainhead,
                        Subhead = model.Subhead

                    };
                    _context.Nearby.Add(welcome);
                }

                else
                {
                    existing.Mainhead = model.Mainhead;
                    existing.Subhead = model.Subhead;

                    if (existing.Image1 != null)
                    {
                        existing.Image1 = imagebytes1;
                    }
                    if (existing.Image2 != null)
                    {
                        existing.Image2 = imagebyte2;
                    }
                    _context.Nearby.Update(existing);
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