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

        public async Task<IActionResult> Index(string country = "us", string category = "general")
        {
            string apiKey = _configuration["NewsApi:ApiKey"];
            string url = $"https://newsapi.org/v2/top-headlines?country={country}&category={category}&apiKey={apiKey}";

            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Грешка при зареждане на новините.";
                ViewBag.Countries = GetCountries();
                ViewBag.Categories = GetCategories();

                return View(new NewsViewModel
                {
                    Articles = new List<NewsArticle>(),
                    SelectedCountry = country,
                    SelectedCategory = category
                });
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<NewsApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            ViewBag.Countries = GetCountries();
            ViewBag.Categories = GetCategories();

            return View(new NewsViewModel
            {
                Articles = result?.Articles ?? new List<NewsArticle>(),
                SelectedCountry = country,
                SelectedCategory = category
            });
        }

        private void SetFiltersInViewBag()
        {
            ViewBag.Countries = new List<string> { "us", "gb", "de", "fr", "bg" };
            ViewBag.Categories = new List<string> { "business", "entertainment", "general", "health", "science", "sports", "technology" };
        }

        public class NewsApiResponse
        {
            public List<NewsArticle> Articles { get; set; }

        }

        private List<string> GetCountries()
        {
            return new List<string>
            {
                "us", "gb", "bg", "de", "fr", "it", "ca", "au"
            };
        }

        private List<string> GetCategories()
        {
            return new List<string>
            {
                "business", "entertainment", "general", "health", "science", "sports", "technology"
            };
        }
    }

}



