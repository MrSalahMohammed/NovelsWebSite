namespace Novels.Domain.Entities
{
    public class AuthorProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public string? Bio { get; set; }
        public ICollection<Novel> Novels { get; set; } = new List<Novel>();
    }
}