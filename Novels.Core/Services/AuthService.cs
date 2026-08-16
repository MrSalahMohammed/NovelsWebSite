using Microsoft.AspNetCore.Identity;
using Novels.Core.Interfaces.Services;
using Novels.Domain.Entities;

namespace Novels.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResult> RegisterAsync(
            string email,
            string password,
            string fName,
            string lName
        )
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FName = fName,
                LName = lName,
            };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return AuthResult.Failure(createResult.Errors.Select(e => e.Description).ToArray());

            await _userManager.AddToRoleAsync(user, Roles.Reader); // default role on self-registration

            return await IssueTokenAsync(user);
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return AuthResult.Failure("Invalid email or password.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true
            );
            if (!signInResult.Succeeded)
                return AuthResult.Failure("Invalid email or password.");

            return await IssueTokenAsync(user);
        }

        private async Task<AuthResult> IssueTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAtUtc) = _tokenService.GenerateToken(user, roles);
            return AuthResult.Success(token, expiresAtUtc, user.Email!, roles.ToArray());
        }
    }
}
