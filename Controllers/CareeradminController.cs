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
    public class CareeradminController : Controller
    {
        private readonly ILogger<CareeradminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CareeradminController(ILogger<CareeradminController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        
                public async Task<IActionResult> Index()
        {
                 var career = await _context.Career.ToListAsync();
            return View("Views/Admin/Career/Index.cshtml" ,career);
        }

         [Route("departments")]
        public async Task<IActionResult> departments()
        {
             var department = await _context.Departments.ToListAsync();
            return View("Views/Admin/Career/departments.cshtml",department);
        }
        
       /*   [Route("designation")]
        
        public IActionResult designation(){
            return View("Views/Admin/Career/designation.cshtml");
        } */
      [Route("welcome")]

  public async Task<IActionResult> welcomes() {
    var data = _context.CareerWelcome.ToList(); // Fetch CareerWelcomeModel entries
    return View("Views/Admin/Career/welcome.cshtml", data);
}

         [Route("add-career-welcome")]
        [HttpPost]
        public async Task<IActionResult> Addwelcome(
           CareerWelcomeModel model,
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
                var existing = await _context.CareerWelcome.FirstOrDefaultAsync();

                if (existing == null)
                {
                    var welcome = new CareerWelcomeModel
                    {
                        Image1 = imagebytes1,
                        Image2 = imagebyte2,
                        Mainhead = model.Mainhead,
                        Subhead = model.Subhead

                    };
                    _context.CareerWelcome.Add(welcome);
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
                    _context.CareerWelcome.Update(existing);
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