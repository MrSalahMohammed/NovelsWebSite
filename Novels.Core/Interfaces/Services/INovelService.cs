using Novels.Core.DTOs.Novel;

namespace Novels.Core.Interfaces.Services
{
    public interface INovelService
    {
        Task<bool> AddChapterToNovel(AddChapterRequest request, int userId);
    }
}
