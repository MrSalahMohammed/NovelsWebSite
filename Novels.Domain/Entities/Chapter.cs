namespace Novels.Domain.Entities
{
    public class Chapter
    {
        public int Id { get; set; }
        public int NovelId { get; set; }
        public Novel Novel { get; set; } = null!;

        public int ChapterNumber { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsPublished { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}