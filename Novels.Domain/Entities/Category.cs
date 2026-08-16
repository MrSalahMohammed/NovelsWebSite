namespace Novels.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Novel> Novels { get; set; } = new List<Novel>();
    }
}