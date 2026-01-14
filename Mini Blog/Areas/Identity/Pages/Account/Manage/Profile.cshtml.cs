using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniBlog.Models;

namespace MiniBlog.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string? AvatarUrl { get; set; }

        [BindProperty]
        public IFormFile? Avatar { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            AvatarUrl = user?.AvatarUrl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (Avatar != null && Avatar.Length > 0)
            {
                var uploads = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/avatars");

                Directory.CreateDirectory(uploads);

                var fileName = $"{user.Id}{Path.GetExtension(Avatar.FileName)}";
                var filePath = Path.Combine(uploads, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await Avatar.CopyToAsync(stream);

                user.AvatarUrl = "/avatars/" + fileName;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToPage();
        }
    }
}
