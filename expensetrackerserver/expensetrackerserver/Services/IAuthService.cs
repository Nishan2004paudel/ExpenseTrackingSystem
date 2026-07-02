using expensetrackerserver.DTOs;
namespace expensetrackerserver.Services
{
    public interface IAuthService
    {
        Task<UserDetailDto> Register(RegisterUserDto dto);
        Task<LoginResponseDto> Login(LoginDto dto);
        Task<UserDetailDto> GetUserDetail(int userId);
    }
}
