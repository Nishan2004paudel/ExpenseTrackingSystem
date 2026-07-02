
using Microsoft.AspNetCore.Mvc;
using expensetrackerserver.Services;
using expensetrackerserver.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace expensetrackerserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
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
    }
}
