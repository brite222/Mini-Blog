using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniBlog.Data;
using MiniBlog.Models;

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
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return NotFound();

            return View(post);
        }

        // =========================
        // CREATE (ADMIN ONLY)
        // =========================
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost post)
        {
            if (!ModelState.IsValid)
                return View(post);

            post.CreatedAt = DateTime.UtcNow;

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
    }
}
