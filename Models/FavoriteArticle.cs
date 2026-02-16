using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NewsExplorerApp.Models
{
    public class FavoriteArticle
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Url { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Source { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }

}

