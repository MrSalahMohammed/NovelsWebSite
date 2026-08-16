namespace Novels.Domain.Entities
{
    public class Novel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public NovelStatus Status { get; set; } = NovelStatus.Ongoing;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int AuthorProfileId { get; set; }
        public AuthorProfile AuthorProfile { get; set; } = null!;

        public decimal AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public int Viewers { get; set; }
        public int NumOfCollections { get; set; }

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<ApplicationUser> FavoritedBy { get; set; } = new List<ApplicationUser>();
    }
}