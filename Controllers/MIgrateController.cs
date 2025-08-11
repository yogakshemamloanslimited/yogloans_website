using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yogloansdotnet.Data;

public class MigrateController : Controller
{
    private readonly ApplicationDbContext _context;

    public MigrateController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Route("migrate")]
    public IActionResult Migrate()
    {
        try
        {
            // Drop the database if it exists
            _context.Database.EnsureDeleted();

            // Recreate the database and apply all migrations
            _context.Database.Migrate();

            return Content("✅ Database dropped and migrated successfully.");
        }
        catch (Exception ex)
        {
            return Content("❌ Migration failed: " + ex.Message);
        }
    }
}
