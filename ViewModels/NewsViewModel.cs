using NewsExplorerApp.Models;

namespace NewsExplorerApp.ViewModels
{
    public class NewsViewModel
    {
        public List<NewsArticle> Articles { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedCountry { get; set; }

        public List<string> Categories { get; set; } = new()
        {
            "business", "entertainment", "general", "health", "science", "sports", "technology"
        };

        public List<string> Countries { get; set; } = new()
        {
            "us", "gb", "de", "fr", "it", "bg"
        };
    }
}
