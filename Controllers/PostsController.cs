using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.View_Models;
using BlogProject.Services;
using X.PagedList.Extensions;
using X.PagedList.EF;
using AspNetCoreGeneratedDocument;

namespace BlogProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly BlogSearchService _blogSearchService;

        public PostsController(
            ApplicationDbContext context,
            ISlugService slugService,
            IImageService imageService,
            UserManager<BlogUser> userManager,
            BlogSearchService blogSearchService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
        }

        /* FEATURE : Add TagIndex action and View in the PostsController to display an index off all posts with a related tag (use searchIndex action/view for reference) */

        // GET : Posts/SearchIndex
        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;

            var pageNumber = page ?? 1;
            var pageSize = 5;

            var posts = _blogSearchService.Search(searchTerm);

           
            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Posts/Index
        public async Task<IActionResult> Index() // Index of ALL Posts
        {
            var applicationDbContext = _context.Posts.Include(p => p.Blog).Include(p => p.BlogUser);
            return View(await applicationDbContext.ToListAsync());
        }

        //GET: Posts/BlogPostIndex
        public async Task<IActionResult> BlogPostIndex(int? id, int? page) // Index of Posts in Blog
        {
            if (id == null)
            {
                return NotFound();
            }

            var pageNumber = page ?? 1;
            var pageSize = 6;

            var posts = _context.Posts
                .Where(p => p.BlogId == id)
                .OrderByDescending(p => p.Created);

            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            ViewData["Title"] = "Post Details Page";
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser) // This BlogUser is the Author of the Post
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser) // This is the Author of the Comment
                .Include(p => p.Comments)
                .ThenInclude(c => c.Moderator) // This is the Moderator of the Comment
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null) return NotFound();

            var dataVM = new PostDetailViewModel()
            {
                Post = post,
                Tags = _context.Tags
                    .Select(t => t.Text.ToLower())
                    .Distinct().ToList()
            };

            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData, post.Content);
            ViewData["MainText"] = post.Title;
            ViewData["SubText"] = post.Abstract;

            return View(dataVM);
        }

        //public async Task<IActionResult> Details(string slug)
        //{
        //    if (slug == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts
        //        .Include(p => p.Blog)
        //        .Include(p => p.BlogUser) // This BlogUser is the Author of the Post
        //        .Include(p => p.Tags)
        //        .Include(p => p.Comments)
        //        .ThenInclude(c => c.BlogUser) // This is the Author of the Comment
        //        .FirstOrDefaultAsync(m => m.Slug == slug);

        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(post);
        //}

        // GET: Posts/Create
        public IActionResult Create(int? id)
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                var authorId = _userManager.GetUserId(User);
                post.BlogUserId = authorId;

                // Use the _imageService to store the user uploaded image data
                post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                post.ContentType = _imageService.ContentType(post.Image);

                // Create the slug and determine if it is unique
                var slug = _slugService.UrlFriendly(post.Title);
                // Create a variable to store whether an error has occured
                var validationErrorOccurred = false;

                // Detect incoming duplicate slugs (and modify the incoming slug to be unique???)
                if (string.IsNullOrEmpty(slug))
                {
                    validationErrorOccurred = true;
                    // Add a Model state error and send back to the view
                    ModelState.AddModelError("", "The title you provided results in an empty slug. Please provide a unique title");
                }

                else if (!_slugService.IsUnique(slug))
                {
                    validationErrorOccurred = true;
                    // Add a Model state error and send back to the view
                    ModelState.AddModelError("Title", "The title you provided results in a duplicate slug. Please provide a unique title");
                }


                //// Add custom model errors as needed
                //else if (slug.Contains("test"))
                //{
                //    validationErrorOccurred = true;
                //    // Add a Model state error and send back to the view
                //        // Errors are tied to the property indicated in the first parameter, w/empty string appearing at the top of the view as an unordered list
                //    ModelState.AddModelError("", "Uh-oh! It looks like you are trying to test the slug service. Please provide a unique title");
                //    ModelState.AddModelError("Title", "The title you provided contains the word 'test'. Please provide a unique title");
                    
                //}

                if (validationErrorOccurred)
                {
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();

                // Add each new tag from the incoming list to the database
                foreach (var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tagText
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("BlogPostIndex", "Posts", new { id = post.BlogId });
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus")] Post post, IFormFile? newImage, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get copy of original post from the database to preserve data that does not change, and update the data that does in the edit view 
                    var newPost = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
                    newPost.Updated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    newPost.BlogId = post.BlogId;
                    newPost.Title = post.Title;
                    newPost.Abstract = post.Abstract;
                    newPost.Content = post.Content;
                    newPost.ReadyStatus = post.ReadyStatus;


                    var newSlug = _slugService.UrlFriendly(post.Title);

                    if (newSlug != newPost.Slug)
                    {
                        if (_slugService.IsUnique(newSlug))
                        {
                            newPost.Title = post.Title;
                            newPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "This Title results in a duplicate slug. Please provide a unique title");
                            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));

                            return View(post);
                        }

                    }

                    if (newImage is not null)
                    {
                        newPost.ImageData = await _imageService.EncodeImageAsync(newImage);
                        newPost.ContentType = _imageService.ContentType(newImage);
                    }

                    // Remove all Tags previously associated with this Post
                    _context.Tags.RemoveRange(newPost.Tags);

                    // Add each new tag from the Edit form to the database
                    foreach (var tagText in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = newPost.BlogUserId,
                            Text = tagText
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("BlogPostIndex", "Posts", new { id = post.BlogId });           
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);

            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        /* FEATURE : This is a Hard Delete. Refactor to create a Soft Delete using a bool check to remove it from the view but not from the database, if desired */

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
