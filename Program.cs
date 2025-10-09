using Microsoft.EntityFrameworkCore;
using yogloansdotnet.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

// ✅ Add distributed memory cache (needed for sessions)
builder.Services.AddDistributedMemoryCache();

// ✅ Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // Helps during redirects
});

// ✅ Add CORS (if your AJAX calls come from another port or domain)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // ✅ Allow credentials for sessions to work across AJAX calls
        policy
            .WithOrigins("http://localhost:8085", "https://localhost:5001") // Adjust to your actual domains
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ✅ Register EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Authentication setup
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/admin/Login";
        options.LogoutPath = "/admin/Logout";
        options.AccessDeniedPath = "/admin/AccessDenied";
    })
    .AddCookie("AuctionCookie", options =>
    {
        options.LoginPath = "/Auction/Login";
        options.LogoutPath = "/Auction/Logout";
        options.AccessDeniedPath = "/Auction/Login";
    });

var app = builder.Build();

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ✅ Correct middleware order
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAll");   // Allow AJAX cross-origin access (if needed)
app.UseSession();          // ✅ Enable Session before Auth
app.UseAuthentication();
app.UseAuthorization();

// ✅ Default route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
