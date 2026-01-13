using Microsoft.AspNetCore.Mvc;
using MiniBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace MiniBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 6;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(
    string? search,
    int? categoryId,
    int? tagId,
    int page = 1)
        {
            const int PageSize = 6;

            var query = _context.BlogPosts
                .Include(p => p.Category)
                .Include(p => p.BlogPostTags)
                    .ThenInclude(bt => bt.Tag)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Title.Contains(search) ||
                    p.Content.Contains(search));
            }

            // CATEGORY FILTER
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // TAG FILTER
            if (tagId.HasValue)
            {
                query = query.Where(p =>
                    p.BlogPostTags.Any(bt => bt.TagId == tagId));
            }

            var totalPosts = await query.CountAsync();

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // FOR VIEW
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.TagId = tagId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages =
                (int)Math.Ceiling(totalPosts / (double)PageSize);

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();

            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(Array.Empty<object>());

            var results = await _context.BlogPosts
                .Where(p => p.Title.Contains(term))
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    id = p.Id,
                    title = p.Title
                })
                .Take(5)
                .ToListAsync();

            return Json(results);
        }


        public IActionResult Privacy()
        {
            return View();
        }
    }
}
