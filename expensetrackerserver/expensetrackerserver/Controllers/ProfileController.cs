using expensetrackerserver.DTOs;
using expensetrackerserver.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace expensetrackerserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _service;


        public ProfileController(IProfileService service)
        {
            _service = service;

        }

        [Authorize]
        [HttpPut("preferred-calendar")]
        public async Task<IActionResult> ChangePreferredCalendar(UpdatePreferredCalendarDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.ChangePreferredCalendar(userId, dto);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("setup-password")]
        public async Task<IActionResult> SetupPassword(SetupPasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.SetupPassword(userId, dto);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("setup-username")]
        public async Task<IActionResult> SetupUsername(SetupUsernameDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.SetupUsername(userId, dto);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _service.GetUserDetail(userId);
            return Ok(user);
        }

        [Authorize]
        [HttpPut("full-name")]
        public async Task<IActionResult> UpdateFullName(UpdateFullNameDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.UpdateFullName(userId, dto);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("profession")]
        public async Task<IActionResult> UpdateProfession(UpdateProfessionDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.UpdateProfession(userId, dto);
            return Ok(response);
        }

    }
}
