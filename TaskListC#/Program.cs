using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskListC_.Context;
using TaskListC_.Helpers;
using TaskListC_.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<User, IdentityRole>()
  .AddEntityFrameworkStores<AppDbContext>()
  .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserHelper>();

builder.Services.Configure<IdentityOptions>(options =>
{
  options.Password.RequireDigit = false;
  options.Password.RequireLowercase = false;
  options.Password.RequireUppercase = false;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequiredLength = 3;
  options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(60);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
