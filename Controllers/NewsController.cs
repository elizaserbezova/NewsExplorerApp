using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NewsExplorerApp.Models;
using NewsExplorerApp.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewsExplorerApp.Controllers
{
    public class NewsController : Controller
    {
        private readonly IConfiguration _configuration;

        public NewsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string country = "us", string category = "general", string searchQuery = "", string sources = "", string sortOrder = "desc")
        {
            string apiKey = _configuration["NewsApi:ApiKey"];
            string url = "https://newsapi.org/v2/top-headlines"; 

            var urlBuilder = new UriBuilder(url);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query["q"] = System.Web.HttpUtility.UrlEncode(searchQuery); 
            }

            
            if (!string.IsNullOrEmpty(sources))
            {
                query["sources"] = sources; 
            }
            else
            {           
                if (!string.IsNullOrEmpty(country)) query["country"] = country; 
                if (!string.IsNullOrEmpty(category)) query["category"] = category; 
            }

            query["apiKey"] = apiKey;

            urlBuilder.Query = query.ToString();
            url = urlBuilder.ToString();

            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Error loading news.";
                ViewBag.Countries = GetCountries();
                ViewBag.Categories = GetCategories();
                ViewBag.Sources = await GetSourcesAsync();

                return View(new NewsViewModel
                {
                    Articles = new List<NewsArticle>(),
                    SelectedCountry = country,
                    SelectedCategory = category,
                    SearchQuery = searchQuery,
                    SelectedSources = sources
                });
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<NewsApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var articles = result?.Articles ?? new List<NewsArticle>();

     
            if (sortOrder == "desc")
            {
                articles = articles.OrderByDescending(a => a.PublishedAt).ToList();
            }
            else
            {
                articles = articles.OrderBy(a => a.PublishedAt).ToList();
            }

            ViewBag.Countries = GetCountries();
            ViewBag.Categories = GetCategories();
            ViewBag.Sources = await GetSourcesAsync();

            return View(new NewsViewModel
            {
                Articles = articles,
                SelectedCountry = country,
                SelectedCategory = category,
                SearchQuery = searchQuery,
                SelectedSources = sources,
                SortOrder = sortOrder 
            });
        }



        private List<string> GetCountries()
        {
            return new List<string> { "us", "gb", "de", "fr", "bg", "it", "ca", "au" };
        }

        private List<string> GetCategories()
        {
            return new List<string> { "business", "entertainment", "general", "health", "science", "sports", "technology" };
        }

        private async Task<List<string>> GetSourcesAsync()
        {
            string apiKey = _configuration["NewsApi:ApiKey"];
            string url = $"https://newsapi.org/v2/sources?apiKey={apiKey}";

            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<string>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<NewsApiSourcesResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Sources?.Select(s => s.Id).ToList() ?? new List<string>();
        }

        public class NewsApiSourcesResponse
        {
            public List<NewsApiSource> Sources { get; set; }
        }

        public class NewsApiSource
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }


        public class NewsApiResponse
        {
            public List<NewsArticle> Articles { get; set; }
        }
    }
}