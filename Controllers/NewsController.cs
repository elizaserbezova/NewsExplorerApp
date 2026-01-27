using Microsoft.AspNetCore.Mvc;
using NewsExplorerApp.Models;
using NewsExplorerApp.Services;
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
            ViewBag.Countries = GetCountries();
            ViewBag.Categories = GetCategories();
            ViewBag.Sources = await GetSourcesAsync();

            var vm = await _newsService.GetNewsViewModelAsync(country, category, searchQuery, sources, sortOrder);

            return View(vm);
        }

        private List<string> GetCountries()
            => new() { "us", "gb", "de", "fr", "bg", "it", "ca", "au" };

        private List<string> GetCategories()
            => new() { "business", "entertainment", "general", "health", "science", "sports", "technology" };

        private async Task<List<string>> GetSourcesAsync()
        {
            var result = await _newsApiClient.GetSourcesAsync();
            return result?.Sources?.Select(s => s.Id).ToList() ?? new List<string>();
        }
    }
}