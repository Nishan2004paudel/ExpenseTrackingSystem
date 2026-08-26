using Dapper;
using expensetrackerserver.Data;
using expensetrackerserver.DTOs;

namespace expensetrackerserver.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DapperContext _context;
        public AdminRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminUserDto>> GetAllUsers()
        {
            var sql = @"SELECT UserId, Username, Email, FullName, Profession, PreferredCalendar, Role, AuthProvider, IsEmailVerified, IsActive,DeactivatedBy, DeactivatedAt,DeactivationReason, CreatedAt, UpdatedAt 
                        FROM [User] ORDER BY CreatedAt DESC;";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<AdminUserDto>(sql);
        }
        public async Task<AdminUserDto?> GetUserById(int userId)
        {
            var sql = @"SELECT UserId, Username, Email, FullName, Profession, PreferredCalendar, Role, AuthProvider, IsEmailVerified, IsActive,DeactivatedBy, DeactivatedAt,DeactivationReason, CreatedAt, UpdatedAt
                            FROM [User] WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AdminUserDto>(sql, new { UserId = userId });
        }

        public async Task DeactivateUser(int userId, int adminUserId)
        {
            var sql = @"UPDATE [User] SET IsActive = 0,DeactivatedBy = @AdminUserId, DeactivatedAt=GETDATE(),DeactivationReason = 'Admin', TokenVersion = TokenVersion + 1, UpdatedAt = GETDATE() WHERE UserId = @UserId AND IsActive= 1;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new { UserId = userId, AdminUserId = adminUserId });
        }

        public async Task ActivateUser(int userId)
        {
            var sql = @"UPDATE [User] SET IsActive = 1,DeactivatedBy = NULL, DeactivatedAt = NULL ,DeactivationReason= NULL,TokenVersion = TokenVersion + 1, UpdatedAt = GETDATE() WHERE UserId = @UserId AND IsActive = 0;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new { UserId = userId });
        }

        public async Task DeleteUser(int userId)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync(
                    "DELETE FROM RefreshToken WHERE UserId = @UserId",
                    new { UserId = userId },
                    transaction
                    );

                await connection.ExecuteAsync(
                    "DELETE FROM Expense WHERE UserId = @UserId",
                    new { UserId = userId },
                    transaction);

                await connection.ExecuteAsync(
                    "DELETE FROM BudgetLimit WHERE UserId = @UserId",
                    new { UserId = userId },
                    transaction);

                await connection.ExecuteAsync(
                    "DELETE FROM Category WHERE UserId = @UserId",
                    new { UserId = userId },
                    transaction);

                await connection.ExecuteAsync(
                    "DELETE FROM [User] WHERE UserId = @UserId",
                    new { UserId = userId },
                    transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
