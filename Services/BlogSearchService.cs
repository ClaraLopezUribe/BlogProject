﻿using BlogProject.Data;
using BlogProject.Models;
using Microsoft.EntityFrameworkCore;

/* TODO : Uncomment all .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady before publishing BlogProject */

namespace BlogProject.Services
{
    public class BlogSearchService
    {
        private readonly ApplicationDbContext _context;

        public BlogSearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Post> Search(string searchTerm)
        {
            var posts = _context.Posts.Include(p => p.Tags).AsQueryable();
            //.Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
            if (searchTerm != null)
            {
                searchTerm = searchTerm.ToLower();

                posts = posts.Where(p =>
                    p.Title.ToLower().Contains(searchTerm) ||
                    p.Abstract.ToLower().Contains(searchTerm) ||
                    p.Content.ToLower().Contains(searchTerm) ||
                    p.Tags.Any(tag => tag.Text.ToLower().Contains(searchTerm)) || // Fixed issue here
                    p.Comments.Any(c =>
                            c.Body.ToLower().Contains(searchTerm) ||
                            c.ModeratedBody.ToLower().Contains(searchTerm) ||
                            // Search the info of the Author of the comment for Search Term
                            c.BlogUser.FirstName.ToLower().Contains(searchTerm) ||
                            c.BlogUser.LastName.ToLower().Contains(searchTerm) ||
                            c.BlogUser.Email.ToLower().Contains(searchTerm)));
            }

            return posts.OrderByDescending(p => p.Created);
        }
    }
}
