using NewsExplorerApp.Data;
using NewsExplorerApp.Models;
using NewsExplorerApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace NewsExplorerApp.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly ApplicationDbContext _db;

        public FavoritesService(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(string userId, string url, string? title, string? source, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Missing user.", nameof(userId));

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Missing url.", nameof(url));

            url = url.Trim();

            var exists = await _db.FavoriteArticles.AnyAsync(x => x.UserId == userId && x.Url == url, ct);
            if (exists) return; 

            _db.FavoriteArticles.Add(new FavoriteArticle
            {
                UserId = userId,
                Url = url,
                Title = title?.Trim(),
                Source = source?.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            });

            await _db.SaveChangesAsync(ct);
        }

        public async Task RemoveAsync(string userId, string url, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Missing user.", nameof(userId));

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Missing url.", nameof(url));

            url = url.Trim();

            var fav = await _db.FavoriteArticles
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Url == url, ct);

            if (fav is null) return;

            _db.FavoriteArticles.Remove(fav);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<FavoriteArticle>> ListAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Missing user.", nameof(userId));

            return await _db.FavoriteArticles
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(ct);
        }

        public Task<bool> ExistsAsync(string userId, string url, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(url))
                return Task.FromResult(false);

            url = url.Trim();
            return _db.FavoriteArticles.AnyAsync(x => x.UserId == userId && x.Url == url, ct);
        }

        public async Task<HashSet<string>> GetFavoriteUrlsAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Missing user.", nameof(userId));

            var urls = await _db.FavoriteArticles
                .Where(x => x.UserId == userId)
                .Select(x => x.Url)
                .ToListAsync(ct);

            return urls.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }
}
