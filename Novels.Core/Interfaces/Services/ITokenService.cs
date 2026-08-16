using Novels.Domain.Entities;

namespace Novels.Core.Interfaces.Services
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAtUtc) GenerateToken(
            ApplicationUser user,
            IList<string> roles
        );
    }
}
