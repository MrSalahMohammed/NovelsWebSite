namespace Novels.Domain.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Novel> Novels { get; set; } = new List<Novel>();
    }
}