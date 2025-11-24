using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using yogloansdotnet.Data;
using yogloansdotnet.Models;

namespace yogloansdotnet.Controllers
{
    [Authorize]
    [Route("admin/[controller]")]
    public class MissionVissionController : Controller
    {
        private readonly ILogger<MissionVissionController> _logger;
        private readonly ApplicationDbContext _context;

        public MissionVissionController(ILogger<MissionVissionController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ---------------------- MISSION ----------------------

        [Route("addmission")]
        [HttpPost]
        public async Task<IActionResult> AddMission([FromForm] string? header, [FromForm] string? content)
        {
            try
            {
                var existing = await _context.mission.FirstOrDefaultAsync();

                if (existing != null)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE mission SET header = {0}, content = {1}",
                        header, content
                    );
                }
                else
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO mission (header, content) VALUES ({0}, {1})",
                        header, content
                    );
                }

                return Ok(new { message = "Mission saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error while saving mission.", error = ex.Message });
            }
        }

        [Route("getmission")]
        [HttpGet]
        public async Task<IActionResult> GetMission()
        {
            var data = await _context.mission.ToListAsync();
            return Json(data);
        }

        // ---------------------- VISSION ----------------------

        [Route("addvission")]
        [HttpPost]
        public async Task<IActionResult> AddVission([FromForm] string? header, [FromForm] string? content)
        {
            try
            {
                var existing = await _context.vission.FirstOrDefaultAsync();

                if (existing != null)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE vission SET header = {0}, content = {1}",
                        header, content
                    );
                }
                else
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO vission (header, content) VALUES ({0}, {1})",
                        header, content
                    );
                }

                return Ok(new { message = "Vission saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error while saving vission.", error = ex.Message });
            }
        }

        [Route("getvission")]
        [HttpGet]
        public async Task<IActionResult> GetVission()
        {
            var data = await _context.vission.ToListAsync();
            return Json(data);
        }
    }
}
