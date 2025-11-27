using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace yogloansdotnet.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Login()
        {

            int? code = HttpContext.Session.GetInt32("code");

            if (code == 100)
            {
                return View("~/Views/admin/Login/index.cshtml", new LoginFormModel
                {
                    Username = string.Empty,
                    Password = string.Empty
                });
            }

            else
            {
                return View("~/Views/_404/index.cshtml");
             
            }
               
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/admin/Login/index.cshtml", model);
            }

            // Find the admin user by username
            var adminUser = _context.AdminLogins.FirstOrDefault(a => a.Username == model.Username);

            if (adminUser != null)
            {
                // TODO: Implement proper password verification here
                // You should hash the entered password and compare it with adminUser.Password_hash
                // For now, a placeholder comparison (INSECURE):
                if (model.Password == adminUser.Password_hash)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, adminUser.Username),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    HttpContext.Session.SetInt32("Code", 100);

                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError("", "Invalid username or password");
            model.ErrorMessage = "Invalid username or password";
            return View("~/Views/admin/Login/index.cshtml", model);
        }

        public IActionResult Index()
        {
            var codeValue = HttpContext.Session.GetInt32("code"); // FIXED

            if (codeValue.HasValue && codeValue.Value == 100)
            {
                return View();
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> welcomeget(int id) // use int instead of string
        {
            // Fetch records that match the loan_id
            var homwelcomeData = await _context.Homwelcome
                                               .Where(h => h.loan_id == id)
                                               .ToListAsync();

            // Return as JSON
            return Json(homwelcomeData);
        }
 

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.SetInt32("code", 0);
            return RedirectToAction("Login");
        }
     

    }
} 