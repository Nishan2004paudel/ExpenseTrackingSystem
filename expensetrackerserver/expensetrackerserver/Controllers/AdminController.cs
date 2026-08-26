using expensetrackerserver.DTOs;
using expensetrackerserver.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace expensetrackerserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;

        public AdminController(IAdminService service)
        {
            _service = service;
        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _service.GetUserById(userId);
            return Ok(user);
        }

        [HttpPost("users/{userId}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.DeactivateUser(userId, adminUserId);
            return Ok(new MessageResponseDto
            {
                Message = "User account deactivated successfully."
            });
        }

        [HttpPost("users/{userId}/activate")]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            await _service.ActivateUser(userId);
            return Ok(new MessageResponseDto
            {
                Message = "User account activated successfully."
            });
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.DeleteUser(userId, adminUserId);
            return NoContent();
        }
    }
}
