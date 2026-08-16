using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Novels.Core.DTOs.Auth;
using Novels.Core.Interfaces.Services;

namespace Novels.API.Controllers.Credintials
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;

        public AuthController(
            IAuthService authService,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator
        )
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var validation = await _registerValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
            }

            var result = await _authService.RegisterAsync(
                request.Email,
                request.Password,
                request.FName,
                request.LName
            );

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(
                new AuthResponse(
                    result.Token!,
                    result.ExpiresAtUtc!.Value,
                    result.Email!,
                    result.Roles
                )
            );
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var validation = await _loginValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
            }

            var result = await _authService.LoginAsync(request.Email, request.Password);

            if (!result.Succeeded)
                return Unauthorized(new { errors = result.Errors });

            return Ok(
                new AuthResponse(
                    result.Token!,
                    result.ExpiresAtUtc!.Value,
                    result.Email!,
                    result.Roles
                )
            );
        }

        private static Dictionary<string, string[]> ToErrorDictionary(
            FluentValidation.Results.ValidationResult validation
        )
        {
            return validation
                .Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }
    }
}
