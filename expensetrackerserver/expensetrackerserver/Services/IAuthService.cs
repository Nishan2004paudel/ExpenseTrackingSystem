using expensetrackerserver.DTOs;
namespace expensetrackerserver.Services
{
    public interface IAuthService
    {
        Task<UserDetailDto> Register(RegisterUserDto dto);
        Task<LoginResponseDto> Login(LoginDto dto);
      
        Task<RefreshTokenResponseDto> Refresh(string refreshToken);
        Task Logout(string refreshToken);
        Task LogoutEverywhere(int userId);
        Task VerifyEmail(string token);
        Task ResendVerificationEmail(ResendVerificationEmailDto dto);
        Task ResendVerificationByEmail(ResendVerificationByEmailDto dto);
        Task<LoginResponseDto> GoogleLogin(GoogleLoginDto dto);

        Task<MessageResponseDto> ForgotPassword(ForgotPasswordDto dto);

        Task<MessageResponseDto> ResetPassword(ResetPasswordDto dto);
    }


}
