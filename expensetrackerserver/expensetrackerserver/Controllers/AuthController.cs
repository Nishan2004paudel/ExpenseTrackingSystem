
using Microsoft.AspNetCore.Mvc;
using expensetrackerserver.Services;
using expensetrackerserver.DTOs;


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

        [HttpGet("userdetail/{userId}")]
        public async Task<IActionResult> Detail(int userId)
        {
            var user = await _service.GetUserDetail(userId);
            return Ok(user);
        }
    }
}
