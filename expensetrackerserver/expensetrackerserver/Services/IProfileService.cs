using expensetrackerserver.DTOs;

namespace expensetrackerserver.Services
{
    public interface IProfileService
    {
        Task<MessageResponseDto> SetupPassword(
int userId,
SetupPasswordDto dto);

        Task<MessageResponseDto> SetupUsername(
            int userId,
            SetupUsernameDto dto);

        Task<MessageResponseDto> ChangePreferredCalendar(
            int userId,
            UpdatePreferredCalendarDto dto);

        Task<UserDetailDto> GetUserDetail(int userId);

        Task<MessageResponseDto> UpdateFullName(int userId, UpdateFullNameDto dto);
        Task<MessageResponseDto> UpdateProfession(int userId, UpdateProfessionDto dto);
    }
}
