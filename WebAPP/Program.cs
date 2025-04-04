using Data.Models;
using Microsoft.EntityFrameworkCore;
using WebAPP.Mapping;
using WebAPP.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ProjektRwaContext>(options =>
{
    options.UseSqlServer("Name=ConnectionStrings:ProjektRWA");
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication().AddCookie(builder =>
{
    builder.LoginPath = "/UserDetail/Login";
    builder.AccessDeniedPath = "/UserDetail/AccessDenied";
    builder.LogoutPath = "/UserDetail/Logout";
    builder.SlidingExpiration = true;
    builder.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

builder.Services.AddAutoMapper(typeof(Mapper));
builder.Services.AddScoped<IDiagnostics, Diagnostics>();
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache

builder.Services.AddSession(builder =>
{
    builder.IdleTimeout = TimeSpan.FromMinutes(30);
    builder.Cookie.HttpOnly = true;
    builder.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
