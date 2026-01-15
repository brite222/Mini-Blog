using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniBlog.Data;
using MiniBlog.Models;
using System.Security.Claims;

namespace MiniBlog.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX (PUBLIC)
        // =========================
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var posts = await _context.BlogPosts
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(posts);
        }

        // =========================
        // DETAILS (PUBLIC)
        // =========================
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.BlogPosts
                .Include(p => p.Category)
                .Include(p => p.BlogPostTags)
                    .ThenInclude(bt => bt.Tag)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.PostLikes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return NotFound();

            ViewBag.TotalLikes = post.PostLikes.Count;

            return View(post);
        }


        // =========================
        // CREATE (ADMIN ONLY)
        // =========================
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }




        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    BlogPost post,
    int[] TagIds,
    IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                return View(post);
            }

            post.CreatedAt = DateTime.UtcNow;

            // IMAGE UPLOAD
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await ImageFile.CopyToAsync(stream);

                post.ImageUrl = "/uploads/" + fileName;
            }

            // TAGS
            post.BlogPostTags = new List<BlogPostTag>();
            foreach (var tagId in TagIds)
            {
                post.BlogPostTags.Add(new BlogPostTag
                {
                    TagId = tagId
                });
            }

            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        // =========================
        // EDIT (ADMIN ONLY)
        // =========================
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.BlogPosts
                .Include(p => p.BlogPostTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.SelectedTags = post.BlogPostTags
                .Select(bt => bt.TagId)
                .ToArray();

            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    BlogPost post,
    int[] TagIds,
    IFormFile? ImageFile)
        {
            if (id != post.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // 🔥 THIS FIXES THE CRASH
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.Tags = await _context.Tags.ToListAsync();
                ViewBag.SelectedTags = TagIds ?? Array.Empty<int>();

                return View(post);
            }

            var existingPost = await _context.BlogPosts
                .Include(p => p.BlogPostTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingPost == null)
                return NotFound();

            existingPost.Title = post.Title;
            existingPost.Content = post.Content;
            existingPost.CategoryId = post.CategoryId;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploads = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads");

                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await ImageFile.CopyToAsync(stream);

                existingPost.ImageUrl = "/uploads/" + fileName;
            }

            existingPost.BlogPostTags.Clear();
            if (TagIds != null)
            {
                foreach (var tagId in TagIds)
                {
                    existingPost.BlogPostTags.Add(new BlogPostTag
                    {
                        BlogPostId = existingPost.Id,
                        TagId = tagId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
                return NotFound();

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post != null)
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingLike = await _context.PostLikes
                .FirstOrDefaultAsync(l => l.BlogPostId == postId && l.UserId == userId);

            if (existingLike == null)
            {
                _context.PostLikes.Add(new PostLike
                {
                    BlogPostId = postId,
                    UserId = userId
                });
            }
            else
            {
                _context.PostLikes.Remove(existingLike);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = postId });
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Details", new { id = postId });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = new Comment
            {
                BlogPostId = postId,
                Content = content,
                UserId = userId!,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = postId });
        }


    }

}
