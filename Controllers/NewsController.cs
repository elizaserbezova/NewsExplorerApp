using Microsoft.AspNetCore.Mvc;
using NewsExplorerApp.Models;
using NewsExplorerApp.Services.Interfaces;
using NewsExplorerApp.ViewModels;

namespace NewsExplorerApp.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private readonly INewsApiClient _newsApiClient;

        public NewsController(INewsService newsService, INewsApiClient newsApiClient)
        {
            _newsService = newsService;
            _newsApiClient = newsApiClient;
        }

        public async Task<IActionResult> Index(
            string country = "us",
            string category = "general",
            string searchQuery = "",
            string sources = "",
            string sortOrder = "desc")
        {
            ValidateInputs(ref country, ref category, ref sortOrder, sources);

            ViewBag.Countries = GetCountries();
            ViewBag.Categories = GetCategories();
            ViewBag.Sources = await GetSourcesAsync();

            var vm = await _newsService.GetNewsViewModelAsync(country, category, searchQuery, sources, sortOrder);

            return View(vm);
        }

        private void ValidateInputs(ref string country, ref string category, ref string sortOrder, string sources)
        {
            if (sortOrder != "asc" && sortOrder != "desc")
            {
                ModelState.AddModelError(nameof(sortOrder), "Sort order must be 'asc' or 'desc'.");
                sortOrder = "desc";
            }

            if (!string.IsNullOrWhiteSpace(sources))
                return;

            var allowedCountries = GetCountries();
            var allowedCategories = GetCategories();

            if (!string.IsNullOrWhiteSpace(country) && !allowedCountries.Contains(country))
            {
                ModelState.AddModelError(nameof(country), "Invalid country.");
                country = "us";
            }

            if (!string.IsNullOrWhiteSpace(category) && !allowedCategories.Contains(category))
            {
                ModelState.AddModelError(nameof(category), "Invalid category.");
                category = "general";
            }
        }

        private List<string> GetCountries()
            => new() { "us", "gb", "de", "fr", "bg", "it", "ca", "au" };

        private List<string> GetCategories()
            => new() { "business", "entertainment", "general", "health", "science", "sports", "technology" };

        private async Task<List<string>> GetSourcesAsync()
        {
            var result = await _newsApiClient.GetSourcesAsync();

            if (!result.IsSuccess)
                return new List<string>();

            return result.Data?.Sources?.Select(s => s.Id).ToList() ?? new List<string>();
        }
    }
}
