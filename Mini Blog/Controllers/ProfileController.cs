using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniBlog.Models;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar)
    {
        var user = await _userManager.GetUserAsync(User);

        if (avatar != null && avatar.Length > 0)
        {
            var uploads = Path.Combine("wwwroot/avatars");
            Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + Path.GetExtension(avatar.FileName);
            var path = Path.Combine(uploads, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await avatar.CopyToAsync(stream);

            user.AvatarUrl = "/avatars/" + fileName;
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction("Index");
    }
}
