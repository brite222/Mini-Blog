using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniBlog.Data;
using MiniBlog.Models;
using Microsoft.EntityFrameworkCore;

namespace MiniBlog.Controllers
{
    [Authorize]
    public class LikesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LikesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Like(int blogPostId)

        {
            var userId = User.Identity!.Name!;
            var existingLike = await _context.PostLikes
                .FirstOrDefaultAsync(l => l.BlogPostId == blogPostId && l.UserId == userId);

            if (existingLike != null)
            {
                // Remove like
                _context.PostLikes.Remove(existingLike);
            }
            else
            {
                // Add like
                var like = new PostLike
                {
                    BlogPostId = blogPostId,
                    UserId = userId
                };
                _context.PostLikes.Add(like);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "BlogPosts", new { id = blogPostId });
        }
    }
}
