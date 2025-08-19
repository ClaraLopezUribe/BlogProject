using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.Services;
using BlogProject.View_Models;
using X.PagedList.EF;

namespace BlogProject.Controllers
{
    public class HomeController : Controller
    {
        // LEARN : What is ILogger for?? it isn't used in this controller...delete?
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, IBlogEmailSender emailSender, ApplicationDbContext context)
        {
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            
        }

        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 3;

            
            var blogs = await _context.Blogs
                //TODO : Uncomment the following line to only show blogs with posts that are production ready
                .Where(b => b.Posts.Any(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady)) /*Wrap in if statement to allow admins to see all blogs regardless of ready status*/
                .Include(b => b.BlogUser)
                .Include(b => b.Posts)
                .OrderByDescending(b => b.Created)
                .ToPagedListAsync(pageNumber, pageSize);
            
            var posts = blogs.FirstOrDefault()?.Posts;

            if (ViewData["HeaderImage"] == null)
            {
               ViewData["HeaderImage"] = @Url.Content("~/assets/img/home-bg.jpg");
            }
           
            
            //ViewData["HeaderImage"] = @Url.Content("~/assets/img/home-bg.jpg");
            ViewData["Title"] = "Home";
            ViewData["MainText"] = "Clara-FYIng Thoughts";
            ViewData["Subtext"] = "A Collection of Blogs About Code, Careers, and Creativity";
            
            return View(blogs); 
        }

        public IActionResult About()
        {
            if (ViewData["HeaderImage"] == null)
            {
                ViewData["HeaderImage"] = @Url.Content("~/assets/img/The-Lake.jpg");
            }

            ViewData["Title"] = "About";
            ViewData["MainText"] = "About Me";
            ViewData["Subtext"] = "My Journey. My Story.";



            return View();
        }

        public IActionResult Contact()
        {
            if (ViewData["HeaderImage"] == null)
            {
                ViewData["HeaderImage"] = @Url.Content("~/assets/img/contact-bg.jpg");
            }

            ViewData["Title"] = "Contact Me";
            ViewData["MainText"] = "Let's Connect!";
            ViewData["Subtext"] = "";

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe model)
        {
            // Incorporate the information entered by the user to the model, then leverage the email sender service to send the email
            model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
            await _emailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject, model.Message);
            return RedirectToAction("Index");

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
