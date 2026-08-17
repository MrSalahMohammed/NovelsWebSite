namespace Novels.Core.Interfaces.Services
{
    public interface IAdminService
    {
        Task<bool> ReactivateReaderAsync(int readerId);
    }
}
