using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniBlog.Data;
using MiniBlog.Models;

[Authorize]
public class LikesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public LikesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Like(int blogPostId)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
            return Unauthorized();

        var existingLike = _context.PostLikes
            .FirstOrDefault(l =>
                l.BlogPostId == blogPostId &&
                l.UserId == userId);

        if (existingLike != null)
        {
            // Unlike
            _context.PostLikes.Remove(existingLike);
        }
        else
        {
            // Like
            _context.PostLikes.Add(new PostLike
            {
                BlogPostId = blogPostId,
                UserId = userId
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(
            "Details",
            "BlogPosts",
            new { id = blogPostId });
    }
}
