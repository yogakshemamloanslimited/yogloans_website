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
    public class HomeloanController : Controller
    {
        private readonly ILogger<HomeloanController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeloanController(ILogger<HomeloanController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var viewModel = new LoanPointsViewModel
            {
                Loans = await _context.Loans.ToListAsync(),
                Points = await _context.LoanPoints.ToListAsync()
            };
            return View("/Views/admin/Homeloan/loansection.cshtml", viewModel);
        }
        
        [Route("points")]
        public async Task<IActionResult> points()
        {
            var viewModel = new LoanPointsViewModel
            {
                Loans = await _context.Loans.ToListAsync(),
                Points = await _context.LoanPoints.ToListAsync()
            };
            return View("/Views/admin/Homeloan/points.cshtml", viewModel);
        }

        [HttpPost]
        [Route("point-create")]
        public async Task<IActionResult> Poincreate(LoanPointModel model)
        {
            try
            {
                _logger.LogInformation($"Received form submission. Loan: {model.Loan}, Points: {string.Join(", ", model.Points)}");

                if (string.IsNullOrWhiteSpace(model.Loan))
                {
                    TempData["Error"] = "Loan must be selected";
                    return RedirectToAction("points", "Homeloan");
                }

                if (model.Points == null || !model.Points.Any() || model.Points.Any(string.IsNullOrWhiteSpace))
                {
                    TempData["Error"] = "At least one point must be entered";
                    return RedirectToAction("points", "Homeloan");
                }

                // Create loan points for each point entered
                foreach (var point in model.Points)
                {
                    var loanPoint = new LoanPointModel
                    {
                        Loan = model.Loan,
                        Point = point
                    };
                    _context.LoanPoints.Add(loanPoint);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Points added successfully!";
                return RedirectToAction("points", "Homeloan");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving points");
                TempData["Error"] = "An error occurred while saving points. Please try again.";
                return RedirectToAction("points", "Homeloan");
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Loancreate(LoanModel model, IFormFile icon)
        {
            try
            {
                _logger.LogInformation($"Received form submission. Loanname: {model.Loanname}, Content: {model.Content}");

                // Validate required fields
                if (string.IsNullOrWhiteSpace(model.Loanname))
                {
                    TempData["Error"] = "Loanname is required";
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }

                if (string.IsNullOrWhiteSpace(model.Content))
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

                // Check if we're updating an existing record
                if (model.Id > 0)
                {
                    var existingRecord = await _context.Loans.FindAsync(model.Id);
                    if (existingRecord != null)
                    {
                        _logger.LogInformation("Updating existing record");
                        existingRecord.Loanname = model.Loanname;
                        existingRecord.Content = model.Content;

                        // Handle icon upload
                        if (icon != null)
                        {
                            _logger.LogInformation("Processing icon upload");
                            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            // Delete old icon if exists
                            if (!string.IsNullOrEmpty(existingRecord.icon))
                            {
                                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingRecord.icon.TrimStart('/'));
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                            }

                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + icon.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await icon.CopyToAsync(fileStream);
                            }
                            existingRecord.icon = "/uploads/" + uniqueFileName;
                        }

                        _context.Loans.Update(existingRecord);
                        _logger.LogInformation("Updated record in context");
                        TempData["Success"] = $"{model.Loanname} loan details updated successfully!";
                    }
                    else
                    {
                        TempData["Error"] = "Record not found";
                        return RedirectToAction("Index", "Admin", new { area = "Admin" });
                    }
                }
                else
                {
                    _logger.LogInformation("Creating new record");
                    // Handle icon upload for new record
                    if (icon != null)
                    {
                        _logger.LogInformation("Processing icon upload for new record");
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + icon.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await icon.CopyToAsync(fileStream);
                        }
                        model.icon = "/uploads/" + uniqueFileName;
                    }

                    _context.Loans.Add(model);
                    _logger.LogInformation("Added new record to context");
                    TempData["Success"] = $"{model.Loanname} loan details added successfully!";
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

        [HttpPost]
        [Route("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var loan = await _context.Loans.FindAsync(id);
                if (loan == null)
                {
                    return Json(new { success = false, message = "Loan not found" });
                }

                // Delete the icon file if it exists
                if (!string.IsNullOrEmpty(loan.icon))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, loan.icon.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting loan");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("delete-point/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePoint(int id)
        {
            try
            {
                var point = await _context.LoanPoints.FindAsync(id);
                if (point == null)
                {
                    return Json(new { success = false, message = "Point not found" });
                }

                _context.LoanPoints.Remove(point);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting point");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Route("offer")]
        public async Task<IActionResult> Offer()
        {   
            var viewModel = new LoanPointsViewModel
            {
                Loans = await _context.Loans.ToListAsync(),
                Offer = await _context.Offer.ToListAsync()
            };
            return View("/Views/admin/Homeloan/offer.cshtml", viewModel);
        }

        [HttpPost]
        [Route("offer-create")]
        public async Task<IActionResult> Offercreate(OfferModel model)
        {
            try
            {
                _context.Offer.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Offer added successfully!";
                return RedirectToAction("Offer", "Homeloan");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving offer");
                TempData["Error"] = "An error occurred while saving offer. Please try again.";
                return RedirectToAction("Offer", "Homeloan");
            }
        

    }
    [Route("delete-offer/{id}")]
    [ValidateAntiForgeryToken]
   public async Task<IActionResult> DeleteOffer(int id)
   {
    try
    {
    var offer = await _context.Offer.FindAsync(id);
    if (offer == null)
    {
        return Json(new { success = false, message = "Offer not found" });
    }
    _context.Offer.Remove(offer);
    await _context.SaveChangesAsync();
    return Json(new { success = true });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error deleting offer");
        return Json(new { success = false, message = ex.Message });
    }
   }
}
    
}