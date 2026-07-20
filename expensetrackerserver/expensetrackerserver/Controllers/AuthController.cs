
using Microsoft.AspNetCore.Mvc;
using expensetrackerserver.Services;
using expensetrackerserver.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using expensetrackerserver.Exceptions;

namespace expensetrackerserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly IWebHostEnvironment _environment;

        public AuthController(IAuthService service, IWebHostEnvironment environment)
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var user = await _service.Register(dto);
            return Ok(user);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var response = await _service.Login(dto);

            Response.Cookies.Append(
                "refreshToken",
                response.RefreshToken,
                GetRefreshCookieOptions());
            response.RefreshToken = string.Empty;
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


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new InvalidRefreshTokenException("Refresh token missing.");
            }

            var tokens = await _service.Refresh(refreshToken);

            Response.Cookies.Append(
                "refreshToken",
                tokens.RefreshToken,
       GetRefreshCookieOptions());
            tokens.RefreshToken = string.Empty;
            return Ok(tokens);
        }

        [HttpGet("verify/email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            await _service.VerifyEmail(token);
            return Ok(new MessageResponseDto
            {
                Message = "Email Verified Successfully."
            });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _service.Logout(refreshToken);
            }

            Response.Cookies.Delete(
                "refreshToken",
GetRefreshCookieOptions());
            return NoContent();
        }

        [Authorize]
        [HttpPost("logout/everywhere")]
        public async Task<IActionResult> LogoutEverywhere()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.LogoutEverywhere(userId);

            Response.Cookies.Delete(
                "refreshToken",
GetRefreshCookieOptions());
            return NoContent();
        }


        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerificationEmail(ResendVerificationEmailDto dto)
        {
            await _service.ResendVerificationEmail(dto);
            return Ok(new MessageResponseDto
            {
                Message = "Verification email sent successfully."
            });
        }

        [HttpPost("resend-verification-by-email")]
        public async Task<IActionResult> ResendVerificationByEmail(ResendVerificationByEmailDto dto)
        {
            await _service.ResendVerificationByEmail(dto);
            return Ok(new MessageResponseDto
            {
                Message = "If an unverified account exists for this email, a verification email has been sent."
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginDto dto)
        {
            var response = await _service.GoogleLogin(dto);
            Response.Cookies.Append("refreshToken", response.RefreshToken, GetRefreshCookieOptions());
            response.RefreshToken = string.Empty;
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
        [HttpPut("preferred-calendar")]
        public async Task<IActionResult> ChangePreferredCalendar(UpdatePreferredCalendarDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.ChangePreferredCalendar(userId, dto);
            return Ok(response);
        }
    }
}
