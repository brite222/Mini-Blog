using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniBlog.Data;
using MiniBlog.Models;

namespace MiniBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
                return RedirectToAction("Dashboard");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var uploads = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/avatars");

            Directory.CreateDirectory(uploads);

            var fileName = $"{user.Id}{Path.GetExtension(avatar.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await avatar.CopyToAsync(stream);

            user.AvatarUrl = "/avatars/" + fileName;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Dashboard");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalPosts = await _context.BlogPosts.CountAsync();
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalComments = await _context.Comments.CountAsync();

            return View();
        }
    }
}
