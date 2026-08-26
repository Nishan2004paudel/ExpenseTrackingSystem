using Dapper;
using expensetrackerserver.Data;
using expensetrackerserver.Models;
namespace expensetrackerserver.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DapperContext _context;
        public NotificationRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<Notification> Create(Notification notification)
        {
            var sql = @"INSERT INTO Notification(UserId, Title, Message, IsRead) VALUES (@UserId, @Title, @Message, @IsRead);
                            SELECT NotificationId, UserId, Title, Message, IsRead, CreatedAt FROM Notification WHERE NotificationId = SCOPE_IDENTITY();";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<Notification>(sql, notification);
        }

        public async Task<IEnumerable<Notification>> GetByUserId(int userId)
        {
            var sql = @"SELECT NotificationId, UserId, Title, Message, IsRead, CreatedAt FROM Notification WHERE UserId = @UserId ORDER BY CreatedAt DESC;";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Notification>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Notification>> GetUnreadByUserId(int userId)
        {
            var sql = @"SELECT NotificationId, UserId, Title, Message, IsRead, CreatedAt FROM Notification WHERE UserId = @UserId AND IsRead = 0 ORDER BY CreatedAt DESC;";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Notification>(sql, new { UserId = userId });
        }

        public async Task<Notification?> GetById(int notificationId, int userId)
        {
            var sql = @"SELECT NotificationId, UserId, Title, Message, IsRead, CreatedAt FROM Notification WHERE NotificationId = @NotificationId AND UserId = @UserId;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Notification>(sql, new { NotificationId = notificationId, UserId = userId });
        }
        public async Task MarkAsRead(int notificationId, int userId)
        {
            var sql = @"UPDATE Notification SET IsRead = 1 WHERE NotificationId = @NotificationId AND UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                NotificationId = notificationId,
                UserId = userId
            });
        }

        public async Task MarkAllAsRead(int userId)
        {
            var sql = @"UPDATE Notification SET IsRead = 1 WHERE UserId = @UserId AND IsRead = 0;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new { UserId = userId });
        }

    }
}
