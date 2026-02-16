using NewsExplorerApp.Models;

namespace NewsExplorerApp.Services.Interfaces
{
    public interface IFavoritesService
    {
        Task AddAsync(string userId, string url, string? title, string? source, CancellationToken ct = default);
        Task RemoveAsync(string userId, string url, CancellationToken ct = default);
        Task<IReadOnlyList<FavoriteArticle>> ListAsync(string userId, CancellationToken ct = default);
        Task<bool> ExistsAsync(string userId, string url, CancellationToken ct = default);
        Task<HashSet<string>> GetFavoriteUrlsAsync(string userId, CancellationToken ct = default);
    }
}
