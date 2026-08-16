namespace Novels.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string email, string password, string fName, string lName);
        Task<AuthResult> LoginAsync(string email, string password);
    }

    public class AuthResult
    {
        public bool Succeeded { get; init; }
        public string? Token { get; init; }
        public DateTime? ExpiresAtUtc { get; init; }
        public string? Email { get; init; }
        public string[] Roles { get; init; } = Array.Empty<string>();
        public string[] Errors { get; init; } = Array.Empty<string>();

        public static AuthResult Success(
            string token,
            DateTime expiresAtUtc,
            string email,
            string[] roles
        ) =>
            new()
            {
                Succeeded = true,
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                Email = email,
                Roles = roles,
            };

        public static AuthResult Failure(params string[] errors) =>
            new() { Succeeded = false, Errors = errors };
    }
}
