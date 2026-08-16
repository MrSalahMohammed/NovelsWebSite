namespace Novels.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int NovelId { get; set; }
        public Novel Novel { get; set; } = null!;
        public int ReaderId { get; set; }
        public ApplicationUser Reader { get; set; } = null!;

        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}