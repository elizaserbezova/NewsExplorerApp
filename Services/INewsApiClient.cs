using NewsExplorerApp.Models;

namespace NewsExplorerApp.Services
{
    public interface INewsApiClient
    {
        Task<NewsApiResult<NewsApiResponse>> GetTopHeadlinesAsync(
           string country,
           string category,
           string searchQuery,
           string sources,
           CancellationToken cancellationToken = default);

        Task<NewsApiResult<NewsApiSourcesResponse>> GetSourcesAsync(
            CancellationToken cancellationToken = default);
    }
}
