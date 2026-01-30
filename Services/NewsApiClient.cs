using Microsoft.Extensions.Options;
using NewsExplorerApp.Models;
using NewsExplorerApp.Options;
using System;
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

        public async Task<NewsApiResult<NewsApiResponse>> GetTopHeadlinesAsync(
            string country,
            string category,
            string searchQuery,
            string sources,
            CancellationToken cancellationToken = default)
        {
            var url = BuildTopHeadlinesUrl(country, category, searchQuery, sources);

            using var response = await _http.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return NewsApiResult<NewsApiResponse>.Fail(response.StatusCode);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var data = JsonSerializer.Deserialize<NewsApiResponse>(json, JsonOptions)
                       ?? new NewsApiResponse { Articles = new List<NewsArticle>() };

            return NewsApiResult<NewsApiResponse>.Success(data);
        }

        public async Task<NewsApiResult<NewsApiSourcesResponse>> GetSourcesAsync(
            CancellationToken cancellationToken = default)
        {
            var url = $"sources?apiKey={Uri.EscapeDataString(_options.ApiKey)}";

            using var response = await _http.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return NewsApiResult<NewsApiSourcesResponse>.Fail(response.StatusCode);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var data = JsonSerializer.Deserialize<NewsApiSourcesResponse>(json, JsonOptions)
                       ?? new NewsApiSourcesResponse { Sources = new List<NewsApiSource>() };

            return NewsApiResult<NewsApiSourcesResponse>.Success(data);
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

