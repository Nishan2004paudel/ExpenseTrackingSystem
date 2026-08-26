using expensetrackerserver.DTOs;
namespace expensetrackerserver.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetMyNotifications(int userId);
        Task<IEnumerable<NotificationDto>> GetMyUnreadNotifications(int userId);
        Task MarkAsRead(int notificationId, int userId);
        Task MarkAllAsRead(int userId);
        Task CreateNotification(int userId, string title, string message);
    }
}
