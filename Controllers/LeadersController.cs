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
   LeadersModel model,
  
    IFormFile Profile
  )
        {
            try
            {
                byte[]? prfilebyte = null;

                if (Profile != null && Profile.Length > 0)
                {
                    using var sm = new MemoryStream();
                    await Profile.CopyToAsync(sm);
                    prfilebyte = sm.ToArray();
                }

                if (model.Id == 0)
                {
                    var director = new LeadersModel()
                    {
                        Name = model.Name,
                        Post = model.Post,
                        About = model.About,
                        Profile = prfilebyte
                    };

                    _context.Leaders.Add(director);
                }
                else
                {
                    var existing = await _context.Leaders.FirstOrDefaultAsync(a => a.Id == model.Id);
                    if (existing != null)
                    {
                        existing.About = model.About;
                        existing.Post = model.Post;
                        existing.Name = model.Name;

                        if (prfilebyte != null)
                        {
                            existing.Profile = prfilebyte;

                        }

                        _context.Leaders.Update(existing);
                    }
                }
                await _context.SaveChangesAsync();
                return Json(new
                {
                    Success = true,
                    Message = "Leaders details saved successfully"
                });


            }
            catch (Exception ex)
    {
        _logger.LogError(ex, "Error saving Leaders");
        return Json(new { success = false, message = "An error occurred while saving the Leaders. Please try again." });
    }
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