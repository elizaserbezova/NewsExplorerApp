using NewsExplorerApp.Models;

namespace NewsExplorerApp.Services
{
    public interface INewsApiClient
    {
        Task<NewsApiResponse?> GetTopHeadlinesAsync(
            string country,
            string category,
            string searchQuery,
            string sources,
            CancellationToken cancellationToken = default);

        Task<NewsApiSourcesResponse?> GetSourcesAsync(
            CancellationToken cancellationToken = default);
    }
}
