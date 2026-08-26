using expensetrackerserver.DTOs;
namespace expensetrackerserver.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminUserDto>> GetAllUsers();
        Task<AdminUserDto> GetUserById(int userId);
        Task DeactivateUser(int userId, int adminUserId);
        Task ActivateUser(int userId);
        Task DeleteUser(int userId,int adminUserId);
    }
}
