namespace Novels.Core.DTOs.Reader
{
    public record NovelSummaryResponse(
        int Id,
        string Name,
        string Slug,
        decimal AverageRating,
        string? CoverImageUrl
    );

    public record ReadingProgressResponse(
        int NovelId,
        int LastChapterId,
        string LastChapterName,
        DateTime LastReadAt
    );

    public record UpdateProgressRequest(int ChapterId);

    public record ReviewRequest(int Score, string? Comment);

    public record ReviewResponse(int NovelId, int Score, string? Comment, DateTime CreatedAt);

    public record UpdateReaderRequest(
        string FName,
        string LName,
        string Email,
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

    public record ReadingProgressDto(
        int Id,
        int NovelId,
        string NovelName,
        string NovelSlug,
        string? NovelCoverImageUrl,
        int LastChapterId,
        string LastChapterName,
        int LastChapterNumber,
        DateTime LastReadAt
    );

    public record ReviewDto(
        int Id,
        int NovelId,
        int ReaderId,
        string ReaderName,
        string? ReaderAvatarUrl,
        int Score,
        string? Comment,
        DateTime CreatedAt
    );
}
