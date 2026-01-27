namespace NewsExplorerApp.Options
{
    public class NewsApiOptions
    {
        public const string SectionName = "NewsApi";

        public string BaseUrl { get; set; } = "https://newsapi.org/v2/";
        public string ApiKey { get; set; } = string.Empty;
    }
}
