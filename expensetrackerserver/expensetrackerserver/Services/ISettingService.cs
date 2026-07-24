using expensetrackerserver.DTOs;
namespace expensetrackerserver.Services
{
    public interface ISettingService
    {
        Task<MessageResponseDto> ChangeEmail(int userId, ChangeEmailDto dto);
        Task<MessageResponseDto> VerifyEmailChange(string token);
        Task<MessageResponseDto> ChangePassword(int userId, ChangePasswordDto dto);
        Task<MessageResponseDto> ChangeUsername(int userId, ChangeUsernameDto dto);
    }
}
