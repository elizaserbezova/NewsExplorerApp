using NewsExplorerApp.ViewModels;

namespace NewsExplorerApp.Services.Interfaces
{
    public interface INewsService
    {
        Task<NewsViewModel> GetNewsViewModelAsync(
            string country,
            string category,
            string searchQuery,
            string sources,
            string sortOrder,
            CancellationToken cancellationToken = default);

    }
}
