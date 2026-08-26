using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
using expensetrackerserver.Hubs;
using Microsoft.AspNetCore.SignalR;
using expensetrackerserver.Models;
using expensetrackerserver.Repositories;
namespace expensetrackerserver.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(INotificationRepository repo, IHubContext<NotificationHub> hubContext)
        {
            _repo = repo;
            _hubContext = hubContext;
        }
        public async Task<IEnumerable<NotificationDto>> GetMyNotifications(int userId)
        {
            var notifications = await _repo.GetByUserId(userId);
            return notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });
        }
        public async Task<IEnumerable<NotificationDto>> GetMyUnreadNotifications(int userId)
        {
            var notifications = await _repo.GetUnreadByUserId(userId);
            return notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });
        }
        public async Task MarkAsRead(int notificationId, int userId)
        {
            var notification = await _repo.GetById(notificationId, userId);
            if (notification == null)
            {
                throw new InvalidOperationException("Notification not found.");
            }
            if (notification.IsRead)
            {
                return;
            }
            await _repo.MarkAsRead(notificationId, userId);
        }
        public async Task MarkAllAsRead(int userId)
        {
            await _repo.MarkAllAsRead(userId);
        }

        public async Task CreateNotification(int userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false
            };
            var createdNotification = await _repo.Create(notification);
            var notificationDto = new NotificationDto
            {
                NotificationId = createdNotification.NotificationId,
                Title = createdNotification.Title,
                Message = createdNotification.Message,
                IsRead = createdNotification.IsRead,
                CreatedAt = createdNotification.CreatedAt
            };
            await _hubContext.Clients
                .Group($"user-{userId}")
                .SendAsync("ReceiveNotification", notificationDto);
        }
    }
}
