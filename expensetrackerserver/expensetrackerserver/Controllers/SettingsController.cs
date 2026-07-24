using expensetrackerserver.DTOs;
using expensetrackerserver.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace expensetrackerserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _service;
        private readonly IWebHostEnvironment _environment;
        public SettingsController(ISettingService service, IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        private CookieOptions GetRefreshCookieOptions()
        {

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = _environment.IsDevelopment()
                    ? SameSiteMode.None
                    : SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
        }


        [HttpPut("email")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.ChangeEmail(userId, dto);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("verify-email-change")]
        public async Task<IActionResult> VerifyEmailChange([FromQuery] string token)
        {
            var response = await _service.VerifyEmailChange(token);
            return Ok(response);
        }



        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.ChangePassword(userId, dto);
            Response.Cookies.Delete("refreshToken", GetRefreshCookieOptions());
            return Ok(response);
        }

        [HttpPut("username")]
        public async Task<IActionResult> ChangeUsername(ChangeUsernameDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.ChangeUsername(userId, dto);
            return Ok(response);
        }
    }
}
