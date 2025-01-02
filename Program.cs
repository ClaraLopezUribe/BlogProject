using BlogProject.Data;
using BlogProject.Models;
using BlogProject.Services;
using BlogProject.View_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var connectionString = builder.Configuration.GetSection("pgSettings")["pgConnection"];

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDefaultIdentity<BlogUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();


// Register Custom DataServices class
builder.Services.AddScoped<DataService>();

// Register preconfigured instance of MailSettings class
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IBlogEmailSender, EmailService>();

// Register IImageService
builder.Services.AddScoped<IImageService, BasicImageService>();

// Register ISlugService
builder.Services.AddScoped<ISlugService, BasicSlugService>();

var app = builder.Build();


// Get access to registered DataService
var dataService = app.Services.CreateScope()
                     .ServiceProvider.GetRequiredService<DataService>();
// Run initialization ManageDataAsync()
await dataService.ManageDataAsync();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
