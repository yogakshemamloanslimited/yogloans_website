using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using TechTalk.SpecFlow.CommonModels;

namespace yogloansdotnet.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    string Suc_URL;
    string Fail_URL;
    private object merchantId;
    private string requestparams;

    public string MultiAccountInstructionDtls { get; private set; }

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
        var aboutContent = _context.AboutContent.ToList();

        var viewModel = new LoanGroupViewModel
        {
            Gold = goldLoans ?? new List<HomwelcomeModel>(),
            Business = businessLoans ?? new List<HomwelcomeModel>(),
            Vehicle = vehicleLoans ?? new List<HomwelcomeModel>(),
            CD = cdLoans ?? new List<HomwelcomeModel>(),
            AboutContent = aboutContent ?? new List<AboutContentModel>()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetLoanData()
    {
        try
        {
            var loans = await _context.Loans.ToListAsync();
            var loanPoints = await _context.LoanPoints.ToListAsync();

            var loanData = loans.Select(loan => new
            {
                id = loan.Id,
                name = loan.Loanname,
                content = loan.Content,
                icon = loan.icon,
                points = loanPoints.Where(p => p.Loan == loan.Id.ToString())
                                 .Select(p => p.Point)
                                 .ToArray()
            }).ToList();

            return Json(new { success = true, data = loanData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching loan data");
            return Json(new { success = false, message = "Error fetching loan data" });
        }
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



    [HttpGet]
    [Route("Loan-points/{id}")]
    public async Task<IActionResult> GetLoanPoints(string id)
    {
        try
        {
            var loanPoints = await _context.LoanPoints
                .Where(lp => lp.Loan == id) // assuming 'Loan' is a string property in your LoanPoints model
                .ToListAsync();

            return Json(new { success = true, data = loanPoints });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching loan data");
            return Json(new { success = false, message = "Error fetching loan data" });
        }
    }




    [HttpGet]
    [Route("Loan-forall")]
    public async Task<IActionResult> GetLoanPoints()
    {
        try
        {
            var loan = await _context.Loans.ToListAsync();

            return Json(new { success = true, data = loan });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching loan data");
            return Json(new { success = false, message = "Error fetching loan data" });
        }
    }


    [HttpPost]
    public JsonResult SetModuleId(string code, string otp, string mobile)
    {
        if (int.TryParse(code, out int codeValue))
        {
            HttpContext.Session.SetInt32("code", codeValue);

            // If mobile is numeric
            if (int.TryParse(mobile, out int mobileValue))
            {
                HttpContext.Session.SetInt32("mobile", mobileValue);
            }
            else
            {
                HttpContext.Session.SetString("mobile", mobile); // fallback
            }

            // If otp is numeric
            if (int.TryParse(otp, out int otpValue))
            {
                HttpContext.Session.SetInt32("otp", otpValue);
            }
            else
            {
                HttpContext.Session.SetString("otp", otp);
            }

            return Json(new { success = true });
        }
        else
        {
            return Json(new { success = false, message = "Invalid code" });
        }
    }


    [HttpPost]
    public JsonResult auctiondetails(string auctionId, string state_id)
    {


        if (int.TryParse(auctionId, out int auctionIdValue))
        {
            HttpContext.Session.SetInt32("auctionId", auctionIdValue);
        }
        else
        {
            HttpContext.Session.SetString("auctionId", auctionId); // fallback
        }

        // If otp is numeric
        if (int.TryParse(state_id, out int state_idValue))
        {
            HttpContext.Session.SetInt32("state_id", state_idValue);
        }
        else
        {
            HttpContext.Session.SetString("state_id", state_id);
        }

        return Json(new { success = true });


    }

    [HttpPost]
    public JsonResult customerdetail()
    {
        // Store a simple string in session
        HttpContext.Session.SetString("customerdetail", "SomeValue");

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult Customeraddress(string address, string CustomerId)
    {
        try
        {
            if (string.IsNullOrEmpty(CustomerId))
                return Json(new { success = false, message = "CustomerId is missing." });

            // ✅ Make sure no nulls are passed to SetString
            address = address ?? string.Empty;

            HttpContext.Session.SetString("CustomerId", CustomerId);
            HttpContext.Session.SetString("Customeraddress", address);

            return Json(new { success = true, message = "Session data set successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }


    [HttpPost]
    public IActionResult Pantracks(string pan_track_id, string pan)
            {
        try
        {
            if (string.IsNullOrEmpty(pan_track_id))
                return Json(new { success = false, message = "pantrack is missing." });

            // ✅ Make sure no nulls are passed to SetString
            pan_track_id = pan_track_id ?? string.Empty;

            HttpContext.Session.SetString("pantrack", pan_track_id);
            HttpContext.Session.SetString("pan", pan);

            return Json(new { success = true, message = "Session data set successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult CheckSession()
    {
        var val = HttpContext.Session.GetString("Customeraddress") ?? "empty";
        return Content($"Customeraddress = {val}");
    }
    /* [HttpPost]
    public IActionResult SaveSbiSession(string encdata, string encdata1, string orderId)
    {
        HttpContext.Session.SetString("EncryptedParam", encdata);
        HttpContext.Session.SetString("EncryptedParam1", encdata1);
        HttpContext.Session.SetString("orderId", orderId);

        return Json(new {
            sucess = true,
            encdata = encdata,
            encdata1 = encdata1,
            message = "Session values saved successfully." });
    }

     */
    [HttpPost]
    public IActionResult PrepareSbiPayment(string EncryptTrans, string MultiAccountInstructionDtls, string merchIdVal)
    {
        // Save to session or DB
        HttpContext.Session.SetString("EncryptTrans", EncryptTrans);
        HttpContext.Session.SetString("MultiAccountInstructionDtls", MultiAccountInstructionDtls);
        HttpContext.Session.SetString("merchIdVal", merchIdVal);

        // Redirect to a new action that renders the form
        return RedirectToAction("SbiRedirectForm");
    }
    public IActionResult SbiRedirectForm()
    {
        // Retrieve the values from session
        var encryptTrans = HttpContext.Session.GetString("EncryptTrans");
        var multiAccountInstructionDtls = HttpContext.Session.GetString("MultiAccountInstructionDtls");
        var merchIdVal = HttpContext.Session.GetString("merchIdVal");

        // Pass the values to the view
        ViewBag.EncryptTrans = encryptTrans;
        ViewBag.MultiAccountInstructionDtls = multiAccountInstructionDtls;
        ViewBag.MID = merchIdVal;

        return View();
    }



    [HttpGet]
    public IActionResult Payfail(string status, string orderId, string txnId)
    {
       return View("~/Views/payment/failed.cshtml");
    }
    [HttpGet]
    public IActionResult Paysuccess(string status, string orderId, string txnId)
    {
        return View("~/Views/payment/success.cshtml");
    }

    public IActionResult BackgroundOnline(string url)
    {
        try
        {
            if (string.IsNullOrEmpty(url))
                return Json(new { success = false, message = "pantrack is missing." });

            // ✅ Make sure no nulls are passed to SetString
            url = url ?? string.Empty;

            HttpContext.Session.SetString("background_online", url);


            return Json(new { success = true, message = "Session data set successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }

    }


    [HttpPost]
    public IActionResult EncriptData(string amount, string Order, string customerId)
    {
        try
        {
            var request = HttpContext.Request;
            string fullDomain = $"{request.Scheme}://{request.Host}";

            string Suc_URL;
            string Fail_URL;

            if (fullDomain.Contains("www."))
            {
                Suc_URL = $"{fullDomain}/sbionlinepaymentsuccess";
                Fail_URL = $"{fullDomain}/sbionlinepaymentfailure";
            }
            else
            {
                Suc_URL = $"{fullDomain}/sbionlinepaymentsuccess";
                Fail_URL = $"{fullDomain}/sbionlinepaymentfailure";
            }


            string MID = "1001314";
            string Collaborator_Id = "SBIEPAY";
            string Operating_Mode = "DOM";
            string Country = "IN";
            string Currency = "INR";
            string Amount = amount;
            string Order_Number = Order;
            string Other_Details = "Other";
            string cust_id = customerId;
            string pay_mode = "NB";
            string accessmedium = "ONLINE";
            string trancesource = "ONLINE";

            AES256 aes = new AES256();
            string key_Array = "AHGR4Mx0R4WMwuBELDlQ0cXgbOfrxriYen7Ayl2JXmU=";

            string Requestparameter = string.Join("|", new[]
            {
            MID, Operating_Mode, Country, Currency, Amount, Other_Details,
            Suc_URL, Fail_URL, Collaborator_Id, Order_Number,
            cust_id, pay_mode, accessmedium, trancesource
        });

            string EncryptedParam = aes.Encrypt(Requestparameter, key_Array);

            string Request = $"{Amount}|{Currency}|NEFT";
            string EncryptedParam1 = aes.Encrypt(Request, key_Array);

            // Store encrypted data in session
            HttpContext.Session.SetString("EncryptedParam", EncryptedParam);
            HttpContext.Session.SetString("EncryptedParam1", EncryptedParam1);

            // Return JSON directly
            return Json(new
            {
                success = true,
                message = "Encryption successful",
                encryptedParam = EncryptedParam,
                encryptedParam1 = EncryptedParam1
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encryption error");
            return Json(new { success = false, message = ex.Message });
        }
    }

   

    private void OK(object value)
    {
        throw new NotImplementedException();
    }
}
