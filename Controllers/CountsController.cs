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
    public class CountsController : Controller
    {
        private readonly ILogger<CountsController> _logger;
        private readonly ApplicationDbContext _context;

        public CountsController(ILogger<CountsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("")]
        [Route("Counts")]
        public async Task<IActionResult> Index()
        {
            var achievements = await _context.Counts.ToListAsync();
            
            return View("/views/admin/Counts/index.cshtml", achievements);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(CountsModel model)
        {
            if (ModelState.IsValid)
            {
                await _context.Counts.AddAsync(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Achievement added successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Failed to add achievement.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Route("delete-counts/{id}")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> DeleteCounts(int id)
        {
            try
            {
                var Counts = await _context.Counts.FindAsync(id);
                if (Counts == null)
                {
                    return Json(new { success = false, message = "Point not found" });
                }

                _context.Counts.Remove(Counts);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting point");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
    
    }

