using Novels.Domain.Entities;

namespace Novels.Core.Interfaces.Repositories
{
    public interface INovelRepository
    {
        Task<bool> ChapterBelongsToNovelAsync(int chapterId, int novelId);
        Task<bool> NovelExistsAsync(int novelId);
        Task<Novel?> FindNovelByID(int novelId);
        Task RecalculateNovelRatingAsync(int novelId);
        Task SaveChangesAsync();
        Task AddChapterToNovel(Chapter chapter);
        Task<bool> IsNovelBelongsToAuthorAsync(int novelId, int userId);
    }
}
