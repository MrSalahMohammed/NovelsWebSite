namespace Novels.Core.DTOs.Auth
{
    public record RegisterRequest(
        string Email,
        string RecoveryEmail,
        string Password,
        string FName,
        string LName,
        string Phone
    );

    public record LoginRequest(string Email, string Password);

    public record AuthResponse(string Token, DateTime ExpiresAtUtc, string Email, string[] Roles);
}
