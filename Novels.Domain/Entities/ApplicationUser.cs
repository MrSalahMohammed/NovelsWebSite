using Microsoft.AspNetCore.Identity;

namespace Novels.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public string? RecoveryEmail { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AuthorProfile? AuthorProfile { get; set; }

        public ICollection<Novel> FavoriteNovels { get; set; } = new List<Novel>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<ReadingProgress> ReadingHistory { get; set; } = new List<ReadingProgress>();
    }
}