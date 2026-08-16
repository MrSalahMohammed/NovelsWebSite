namespace Novels.Domain.Entities
{
    public class ReadingProgress
    {
        public int Id { get; set; }
        public int ReaderId { get; set; }
        public ApplicationUser Reader { get; set; } = null!;
        public int NovelId { get; set; }
        public Novel Novel { get; set; } = null!;
        public int LastChapterId { get; set; }
        public Chapter LastChapter { get; set; } = null!;
        public DateTime LastReadAt { get; set; } = DateTime.UtcNow;
    }
}