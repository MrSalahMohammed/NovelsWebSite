using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Novels.Core.Interfaces.Services;

namespace Novels.API.Controllers.Admin
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService _AdminService)
        {
            _adminService = _AdminService;
        }

        [HttpPost("{readerId:int}/reactivate")]
        [Authorize(Roles = Domain.Entities.Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ReactivateReader(int readerId)
        {
            if (readerId <= 0)
                return BadRequest(new { message = "Invalid reader ID." });

            var success = await _adminService.ReactivateReaderAsync(readerId);

            if (!success)
            {
                return NotFound(
                    new
                    {
                        message = $"Reader with ID {readerId} was not found or is already active.",
                    }
                );
            }

            return NoContent();
        }
    }
}
