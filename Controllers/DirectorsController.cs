using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow.CommonModels;

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


  [HttpPost]
[ValidateAntiForgeryToken]
[Route("add-director")]
public async Task<IActionResult> addcsr(
   DirectorsModel model,
    IFormFile Profile
   )
{
    try
    {
     byte[]? prfilebyte = null;
        
         if(Profile != null && Profile.Length > 0)
                {
                    using var sm = new MemoryStream();
                    await Profile.CopyToAsync(sm);
                    prfilebyte = sm.ToArray();
                }

        if(model.Id == 0)
                {
                    var director = new DirectorsModel()
                    {
                        Name = model.Name,
                        Post = model.Post,
                        About = model.About,
                        Profile = prfilebyte
                    };

                    _context.Directors.Add(director);
                }
         else
                {
                    var existing = await _context.Directors.FirstOrDefaultAsync(a => a.Id == model.Id);
                    if(existing != null)
                    {
                        existing.About = model.About;
                        existing.Post = model.Post;
                        existing.Name = model.Name;

                        if(prfilebyte != null)
                        {
                            existing.Profile = prfilebyte;
                            
                        }

                        _context.Directors.Update(existing);
                    }
                }
               await _context.SaveChangesAsync();
                return Json(new
                {
                    Success = true,
                    Message = "Director details saved successfully"
                });

               
            }
   catch (Exception ex)
{
    _logger.LogError(ex, "Error saving director");
    return Json(new {
        success = false,
        message = ex.InnerException?.Message ?? ex.Message
    });
}

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