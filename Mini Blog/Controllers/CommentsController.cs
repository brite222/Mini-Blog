using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniBlog.Data;
using MiniBlog.Models;
using System.Security.Claims;

namespace MiniBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE COMMENT
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(
                    "Details",
                    "BlogPosts",
                    new { id = comment.BlogPostId }
                );
            }

            comment.CreatedAt = DateTime.Now;

            // Get logged-in user's email or name
            comment.AuthorName = User.Identity?.Name ?? "Anonymous";

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                "BlogPosts",
                new { id = comment.BlogPostId }
            );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int blogPostId)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(
                "Details",
                "BlogPosts",
                new { id = blogPostId }
            );
        }

    }
}
