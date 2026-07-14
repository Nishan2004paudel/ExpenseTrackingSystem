using expensetrackerserver.DTOs;
namespace expensetrackerserver.Services
{
    public interface IAuthService
    {
        Task<UserDetailDto> Register(RegisterUserDto dto);
        Task<LoginResponseDto> Login(LoginDto dto);
        Task<UserDetailDto> GetUserDetail(int userId);
        Task<RefreshTokenResponseDto> Refresh(string refreshToken);
        Task Logout(string refreshToken);
        Task LogoutEverywhere(int userId);
    }
}
