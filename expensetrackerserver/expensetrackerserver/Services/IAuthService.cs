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
        Task VerifyEmail(string token);
        Task ResendVerificationEmail(ResendVerificationEmailDto dto);
        Task ResendVerificationByEmail(ResendVerificationByEmailDto dto);
        Task<LoginResponseDto> GoogleLogin(GoogleLoginDto dto);
        Task<MessageResponseDto> SetupPassword(
    int userId,
    SetupPasswordDto dto);

        Task<MessageResponseDto> SetupUsername(
            int userId,
            SetupUsernameDto dto);

        Task<MessageResponseDto> ChangePreferredCalendar(
            int userId,
            UpdatePreferredCalendarDto dto);
    }
}
