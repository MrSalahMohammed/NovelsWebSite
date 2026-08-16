namespace Novels.Core.DTOs.Reader
{
    public record NovelSummaryResponse(
        int Id,
        string Name,
        string Slug,
        decimal AverageRating,
        string? CoverImageUrl
    );

    public record ReadingProgressResponse(int NovelId, int LastChapterId, DateTime LastReadAt);

    public record UpdateProgressRequest(int ChapterId);

    public record ReviewRequest(int Score, string? Comment);

    public record ReviewResponse(int NovelId, int Score, string? Comment, DateTime CreatedAt);

    public record UpdateReaderRequest(
        string FName,
        string LName,
        string? RecoveryEmail,
        string? PhoneNumber
    );

    public record PromoteToAuthorRequest(string? Bio);

    public record ReadingHistoryItemResponse(
        int NovelId,
        string NovelName,
        int LastChapterId,
        string LastChapterName,
        DateTime LastReadAt
    );
}
