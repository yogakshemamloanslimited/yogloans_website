using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {   
         var goldLoans = _context.Homwelcome.Where(x => x.LoanType == "Gold").ToList();
    var businessLoans = _context.Homwelcome.Where(x => x.LoanType == "Business").ToList();
    var vehicleLoans = _context.Homwelcome.Where(x => x.LoanType == "Vehicle").ToList();
    var cdLoans = _context.Homwelcome.Where(x => x.LoanType == "CD").ToList();

    var viewModel = new LoanGroupViewModel
    {
        Gold = goldLoans ?? new List<HomwelcomeModel>(),
        Business = businessLoans ?? new List<HomwelcomeModel>(),
        Vehicle = vehicleLoans ?? new List<HomwelcomeModel>(),
        CD = cdLoans ?? new List<HomwelcomeModel>()
    };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> welcomecreate(HomwelcomeModel model, IFormFile Image1, IFormFile Image2)
    {
        try
        {
            _logger.LogInformation($"Received form submission. LoanType: {model.LoanType}, Header: {model.Header}");
            
            // Get existing image paths from form
            var existingImage1 = Request.Form["ExistingImage1"].ToString();
            var existingImage2 = Request.Form["ExistingImage2"].ToString();

            // Validate required fields
            if (string.IsNullOrWhiteSpace(model.Header))
            {
                TempData["Error"] = "Header is required";
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }

            if (string.IsNullOrWhiteSpace(model.SubContent))
            {
                TempData["Error"] = "Content is required";
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }

            // Verify database connection
            try
            {
                await _context.Database.OpenConnectionAsync();
                _logger.LogInformation("Database connection successful");
                await _context.Database.CloseConnectionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection failed");
                TempData["Error"] = "Database connection failed. Please try again.";
                throw;
            }

            // Check if a record with this loan type already exists
            var existingRecord = await _context.Homwelcome.FirstOrDefaultAsync(h => h.LoanType == model.LoanType);
            _logger.LogInformation($"Existing record found: {existingRecord != null}");

            if (existingRecord != null)
            {
                _logger.LogInformation("Updating existing record");
                // Update existing record
                existingRecord.Header = model.Header;
                existingRecord.SubContent = model.SubContent;

                // Handle image uploads
                if (Image1 != null)
                {
                    _logger.LogInformation("Processing Image1 upload");
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName1 = Guid.NewGuid().ToString() + "_" + Image1.FileName;
                    string filePath1 = Path.Combine(uploadsFolder, uniqueFileName1);
                    using (var fileStream = new FileStream(filePath1, FileMode.Create))
                    {
                        await Image1.CopyToAsync(fileStream);
                    }
                    existingRecord.Image1 = "/uploads/" + uniqueFileName1;
                }
                else if (!string.IsNullOrEmpty(existingImage1))
                {
                    // Keep existing image if no new image is uploaded
                    existingRecord.Image1 = existingImage1;
                }

                if (Image2 != null)
                {
                    _logger.LogInformation("Processing Image2 upload");
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName2 = Guid.NewGuid().ToString() + "_" + Image2.FileName;
                    string filePath2 = Path.Combine(uploadsFolder, uniqueFileName2);
                    using (var fileStream = new FileStream(filePath2, FileMode.Create))
                    {
                        await Image2.CopyToAsync(fileStream);
                    }
                    existingRecord.Image2 = "/uploads/" + uniqueFileName2;
                }
                else if (!string.IsNullOrEmpty(existingImage2))
                {
                    // Keep existing image if no new image is uploaded
                    existingRecord.Image2 = existingImage2;
                }

                _context.Homwelcome.Update(existingRecord);
                _logger.LogInformation("Updated record in context");
                TempData["Success"] = $"{model.LoanType} loan details updated successfully!";
            }
            else
            {
                _logger.LogInformation("Creating new record");
                // Handle image uploads for new record
                if (Image1 != null)
                {
                    _logger.LogInformation("Processing Image1 upload for new record");
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName1 = Guid.NewGuid().ToString() + "_" + Image1.FileName;
                    string filePath1 = Path.Combine(uploadsFolder, uniqueFileName1);
                    using (var fileStream = new FileStream(filePath1, FileMode.Create))
                    {
                        await Image1.CopyToAsync(fileStream);
                    }
                    model.Image1 = "/uploads/" + uniqueFileName1;
                }
                else if (!string.IsNullOrEmpty(existingImage1))
                {
                    model.Image1 = existingImage1;
                }

                if (Image2 != null)
                {
                    _logger.LogInformation("Processing Image2 upload for new record");
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName2 = Guid.NewGuid().ToString() + "_" + Image2.FileName;
                    string filePath2 = Path.Combine(uploadsFolder, uniqueFileName2);
                    using (var fileStream = new FileStream(filePath2, FileMode.Create))
                    {
                        await Image2.CopyToAsync(fileStream);
                    }
                    model.Image2 = "/uploads/" + uniqueFileName2;
                }
                else if (!string.IsNullOrEmpty(existingImage2))
                {
                    model.Image2 = existingImage2;
                }

                _context.Homwelcome.Add(model);
                _logger.LogInformation("Added new record to context");
                TempData["Success"] = $"{model.LoanType} loan details added successfully!";
            }

            try
            {
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation($"Database save completed. Affected rows: {result}");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update failed");
                _logger.LogError($"Inner exception: {ex.InnerException?.Message}");
                TempData["Error"] = "Failed to save changes. Please try again.";
                throw;
            }

            return RedirectToAction("Index", "Admin", new { area = "Admin" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing form submission");
            TempData["Error"] = "An error occurred while processing your request. Please try again.";
            return RedirectToAction("Index", "Admin", new { area = "Admin" });
        }
    }
}
