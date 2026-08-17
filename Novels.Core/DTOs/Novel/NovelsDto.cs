namespace Novels.Core.DTOs.Novel
{
    public record NovelDto(
        int Id,
        string Name,
        string Slug,
        string? Description,
        string? CoverImageUrl,
        string Status,
        int AuthorProfileId,
        string AuthorName,
        decimal AverageRating,
        int RatingsCount,
        int Viewers,
        int NumOfCollections,
        List<string> Tags,
        List<string> Categories,
        int ChapterCount
    );

    public record ChapterDto(
        int Id,
        int NovelId,
        int ChapterNumber,
        string Name,
        string Content,
        bool IsPublished,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );

    public record AddChapterRequest(int NovelId, string Name, string Content);
}
