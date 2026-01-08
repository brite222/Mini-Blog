using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniBlog.Data;

namespace MiniBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /
        // Home page displays latest blog posts (READ-ONLY)
        public async Task<IActionResult> Index()
        {
            var posts = await _context.BlogPosts
                                      .OrderByDescending(p => p.CreatedAt)
                                      .ToListAsync();

            return View(posts);
        }

        // Optional Privacy page
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
