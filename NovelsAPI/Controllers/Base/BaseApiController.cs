using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected int CurrentReaderId
    {
        get
        {
            var claimValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (int.TryParse(claimValue, out var readerId))
            {
                return readerId;
            }

            throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
        }
    }
}
