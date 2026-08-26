using expensetrackerserver.Models;
namespace expensetrackerserver.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> Create(Notification notification);
        Task<IEnumerable<Notification>> GetByUserId(int userId);
        Task<IEnumerable<Notification>> GetUnreadByUserId(int userId);
        Task<Notification?> GetById(int notificationId, int userId);
        Task MarkAsRead(int notificationId, int userId);
        Task MarkAllAsRead(int userId);

    }
}
