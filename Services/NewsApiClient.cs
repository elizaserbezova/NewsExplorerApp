using Microsoft.Extensions.Options;
using NewsExplorerApp.Models;
using NewsExplorerApp.Options;
using System.Text.Json;

namespace NewsExplorerApp.Services
{
    public class NewsApiClient : INewsApiClient
    {
        private readonly HttpClient _http;
        private readonly NewsApiOptions _options;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public NewsApiClient(HttpClient http, IOptions<NewsApiOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        public async Task<NewsApiResponse?> GetTopHeadlinesAsync(
            string country,
            string category,
            string searchQuery,
            string sources,
            CancellationToken cancellationToken = default)
        {
            var url = BuildTopHeadlinesUrl(country, category, searchQuery, sources);

            using var response = await _http.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<NewsApiResponse>(json, JsonOptions);
        }

        public async Task<NewsApiSourcesResponse?> GetSourcesAsync(
            CancellationToken cancellationToken = default)
        {
            var url = $"sources?apiKey={Uri.EscapeDataString(_options.ApiKey)}";

            using var response = await _http.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<NewsApiSourcesResponse>(json, JsonOptions);
        }

        private string BuildTopHeadlinesUrl(string country, string category, string searchQuery, string sources)
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(searchQuery))
                query.Add($"q={Uri.EscapeDataString(searchQuery)}");

            if (!string.IsNullOrWhiteSpace(sources))
            {
                query.Add($"sources={Uri.EscapeDataString(sources)}");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(country))
                    query.Add($"country={Uri.EscapeDataString(country)}");

                if (!string.IsNullOrWhiteSpace(category))
                    query.Add($"category={Uri.EscapeDataString(category)}");
            }

            query.Add($"apiKey={Uri.EscapeDataString(_options.ApiKey)}");

            return "top-headlines" + "?" + string.Join("&", query);
        }
    }
}
