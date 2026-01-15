using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniBlog.Data;
using MiniBlog.Models;

[Authorize]
public class CommentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CommentsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int blogPostId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return RedirectToAction("Details", "BlogPosts", new { id = blogPostId });

        var userId = _userManager.GetUserId(User);

        if (userId == null)
            return Unauthorized();

        var comment = new Comment
        {
            BlogPostId = blogPostId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction(
            "Details",
            "BlogPosts",
            new { id = blogPostId });
    }
}
