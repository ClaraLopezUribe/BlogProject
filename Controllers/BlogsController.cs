﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection.Metadata;

namespace BlogProject.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;

        public BlogsController(ApplicationDbContext context, IImageService imageService, UserManager<BlogUser> userManager)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
        }
        /* TODO : Delete list of Blogs Index View and correspondeing IActionResult if not needed */
        // GET: Blogs/Index List (Not the Landing Page)
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.Blogs.Include(b => b.BlogUser);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Image")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.Created = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                blog.BlogUserId = _userManager.GetUserId(User); // Records the logged in user as the Author of this Blog
                blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                blog.ContentType = _imageService.ContentType(blog.Image);
                _context.Add(blog);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index","Home");
            }

            // Error state
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", blog.BlogUserId);
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        /* ENHANCEMENT : Add count of Posts in Blog to Edit View (see #commentsSection as reference) */
       

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Blog blog, IFormFile? newImage)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newBlog = await _context.Blogs.FindAsync(blog.Id);
                    

                    newBlog.Updated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                    if (newBlog.Name != blog.Name)
                    {
                        newBlog.Name = blog.Name;
                    }

                    if (newBlog.Description != blog.Description)
                    {
                        newBlog.Description = blog.Description;
                    }
                    
                    if (newImage is not null)
                    {
                        newBlog.ImageData = await _imageService.EncodeImageAsync(newImage);
                        newBlog.ContentType = _imageService.ContentType(newImage);      
                    }

                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index","Home");
            }

            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", blog.BlogUserId);
            return View(blog);
        }

        

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.BlogUser)
                .Include(b => b.Posts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            // Prevent a blog containing posts from being deleted.
            if (blog.Posts.Count is > 0)
            {
                TempData["ErrorMessage"] = "This blog has one or more posts. Please reassign or delete posts before deleting the blog";

            }

            return View(blog); //goes to deleteConfirmed action
        }


//        if (blog.Posts.Count is not 0)
//            {
//                TempData["ErrorMessage"] = "This blog has one or more posts. Please reassign or delete posts before deleting the blog";
//            }
            
//            if (blog.Posts.Count is not 0)
//            {
//                return RedirectToAction("BlogPostIndex", "Posts", new { id = blog.Id
//});
//            }
//            else
//{
//    return View(blog);
//}
//        }









        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.Posts)
                .FirstOrDefaultAsync(b => b.Id == id);

            // Prevent user from deleting a blog containing one or more posts
            if (blog.Posts.Count is not 0)            
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
