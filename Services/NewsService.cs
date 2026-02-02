using NewsExplorerApp.Models;
using NewsExplorerApp.Services.Interfaces;
using NewsExplorerApp.ViewModels;

namespace NewsExplorerApp.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsApiClient _client;

        private static readonly List<string> AllowedCountries = new() { "us", "gb", "de", "fr", "bg", "it", "ca", "au" };
        private static readonly List<string> AllowedCategories = new() { "business", "entertainment", "general", "health", "science", "sports", "technology" };

        public NewsService(INewsApiClient client)
        {
            _client = client;
        }

        public async Task<NewsViewModel> GetNewsViewModelAsync(
            string country,
            string category,
            string searchQuery,
            string sources,
            string sortOrder,
            CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(sources))
            {
                country = "";
                category = "";
            }

            sortOrder = (sortOrder == "asc" || sortOrder == "desc") ? sortOrder : "desc";

            var apiResult = await _client.GetTopHeadlinesAsync(country, category, searchQuery, sources, cancellationToken);

            var articles = apiResult?.Data?.Articles ?? new List<NewsArticle>();


            articles = sortOrder == "asc"
                ? articles.OrderBy(a => a.PublishedAt).ToList()
                : articles.OrderByDescending(a => a.PublishedAt).ToList();

            return new NewsViewModel
            {
                Articles = articles,
                SelectedCountry = country,
                SelectedCategory = category,
                SearchQuery = searchQuery,
                SelectedSources = sources,
                SortOrder = sortOrder
            };
        }

        public static IReadOnlyList<string> GetAllowedCountries() => AllowedCountries;
        public static IReadOnlyList<string> GetAllowedCategories() => AllowedCategories;
    }
}

