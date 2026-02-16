using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsExplorerApp.Services.Interfaces;

namespace NewsExplorerApp.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly IFavoritesService _favorites;
        private readonly UserManager<IdentityUser> _userManager;

        public FavoritesController(IFavoritesService favorites, UserManager<IdentityUser> userManager)
        {
            _favorites = favorites;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User);
            var items = await _favorites.ListAsync(userId!);
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string url, string? title, string? source, string returnUrl = "/News")
        {
            var userId = _userManager.GetUserId(User);
            await _favorites.AddAsync(userId!, url, title, source);
            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(string url, string returnUrl = "/Favorites/My")
        {
            var userId = _userManager.GetUserId(User);
            await _favorites.RemoveAsync(userId!, url);
            return LocalRedirect(returnUrl);
        }
    }
}
