using BlogProject.Data;
using BlogProject.Helpers;
using BlogProject.Models;
using BlogProject.Services;
using BlogProject.View_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
//var connectionString = configuration.GetConnectionString("DefaultConnection"); ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//var connectionString = configuration.GetSection("pgSettings")["pgConnection"];
var connectionString = ConnectionHelper.GetConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<BlogUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI() /* LEARN : Is this redundant? doesnt AddDefaultIdentity include the default UI?*/
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();


// Register Custom Services
builder.Services.AddScoped<DataService>();
builder.Services.AddScoped<BlogSearchService>();

// Register preconfigured instance of MailSettings
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IBlogEmailSender, EmailService>();

// Register Custom Interface Services
builder.Services.AddScoped<IImageService, BasicImageService>();

builder.Services.AddScoped<ISlugService, BasicSlugService>();


var app = builder.Build();

//// REFERENCE SECTION: This section is for reference only implementation handled starting with the dataService variable
//using (var scope = app.Services.CreateScope()) {
//    var services = scope.ServiceProvider;
//    var context = services.GetRequiredService<ApplicationDbContext>();

//    // Apply any pending migrations and create the database if it doesn't exist
//    await context.Database.MigrateAsync();

//    // Run additional data management tasks
//    await DataHelper.ManageDataAsync(scope.ServiceProvider);
//}

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

// TODO : Check if SlugRoute is configured correctly
// Create new MapControllerRoute to supersede default route using the Slug, sometimes refered to a "Vanity URL", which improves SEO
app.MapControllerRoute(
    name: "SlugRoute",
    pattern: "BlogPosts/UrlFriendly/{slug}",
    defaults: new { controller = "Posts", action = "Details" });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
