using Microsoft.AspNetCore.Mvc;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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
            // If already logged in, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index");
            }
            return View("~/Views/admin/Login/index.cshtml", new LoginFormModel 
            { 
                Username = string.Empty,
                Password = string.Empty
            });
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

                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError("", "Invalid username or password");
            model.ErrorMessage = "Invalid username or password";
            return View("~/Views/admin/Login/index.cshtml", model);
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }

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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
} 