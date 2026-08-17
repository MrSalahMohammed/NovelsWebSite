using Novels.Core.DTOs.Reader;

namespace Novels.Core.Interfaces.Services
{
    public interface IAuthorService
    {
        Task<bool> PromoteToAuthorAsync(int readerId, PromoteToAuthorRequest request);
    }
}
