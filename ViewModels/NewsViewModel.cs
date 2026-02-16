using NewsExplorerApp.Models;
using System.Collections.Generic;

namespace NewsExplorerApp.ViewModels
{
    public class NewsViewModel
    {
        public List<NewsArticle> Articles { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedCountry { get; set; }

        public string SearchQuery { get; set; }

        public string SelectedSources { get; set; }

        public string SortOrder { get; set; }

        public DateTime? PublishedAt { get; set; }

        public List<string> Categories { get; set; } = new()
        {
            "business", "entertainment", "general", "health", "science", "sports", "technology"
        };

        public List<string> Countries { get; set; } = new()
        {
            "us", "gb", "de", "fr", "it", "bg"
        };

        public string? ErrorMessage { get; set; }

        public HashSet<string> FavoriteUrls { get; set; } = new();

    }
}