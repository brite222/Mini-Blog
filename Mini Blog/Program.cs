using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniBlog.Data;
using MiniBlog.Models;

var builder = WebApplication.CreateBuilder(args);

// =========================
// DATABASE
// =========================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString)
);

// =========================
// IDENTITY
// =========================
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// =========================
// MVC + RAZOR PAGES
// =========================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// =========================
// SESSION (FOR ADS, ETC.)
// =========================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =========================
// COOKIE
// =========================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

// =========================
// DATABASE INIT (CORRECT ORDER)
// =========================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();

    // 1️⃣ Apply migrations FIRST
    context.Database.Migrate();

    // 2️⃣ Seed roles
    await DbInitializer.SeedRolesAsync(services);

    // 3️⃣ Seed admin user
    await DbInitializer.SeedAdminAsync(services);

    // 4️⃣ Seed categories & tags
    ApplicationDbContext.SeedData(context);
}

// =========================
// MIDDLEWARE
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ⚠️ Session MUST be before auth
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// =========================
// ROUTES
// =========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
