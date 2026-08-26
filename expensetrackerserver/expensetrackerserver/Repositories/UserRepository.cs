using expensetrackerserver.DTOs;
using expensetrackerserver.Data;
using expensetrackerserver.Models;
using Dapper;
using System.Data;
namespace expensetrackerserver.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDetailDto>> GetAll()
        {
            var sql = "SELECT UserId, Username, Email, FullName,Profession, PreferredCalendar,Role,AuthProvider FROM [User]";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<UserDetailDto>(sql);
        }

        public async Task<User?> GetByEmailOrUsername(string identifier)
        {
            var sql = @"SELECT * FROM [User] WHERE Email = @Identifier OR Username = @Identifier";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                sql, new { Identifier = identifier });
        }

        public async Task<int> Create(User user)
        {
            var sql = @"INSERT INTO [User] (Username,Email,Password, FullName,Profession, PreferredCalendar,Role,AuthProvider,IsEmailVerified,EmailVerificationToken,EmailVerificationExpiresAt) VALUES ( @Username, @Email,@Password, @FullName, @Profession, @PreferredCalendar,@Role,@AuthProvider,@IsEmailVerified,@EmailVerificationToken,@EmailVerificationExpiresAt);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, user);
        }

        public async Task<User?> GetById(int id)
        {
            var sql = @"SELECT * FROM [User] WHERE UserId = @UserId";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                sql, new { UserId = id });
        }

        public async Task<bool> EmailExists(string email)
        {
            var sql = @"SELECT COUNT(1) FROM [User] WHERE Email = @Email";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                sql, new { Email = email });

            return count > 0;

        }
        public async Task IncrementTokenVersion(int userId)
        {
            var sql = @"UPDATE [User] SET TokenVersion = TokenVersion + 1, UpdatedAt = GETDATE() WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql,
                new
                {
                    UserId = userId
                });
        }

        public async Task<bool> UsernameExists(string username)
        {
            var sql = @"SELECT COUNT(1) FROM [User] WHERE Username = @Username";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                sql, new { Username = username });

            return count > 0;

        }

        public async Task<int?> GetTokenVersion(int userId)
        {
            var sql = @"SELECT TokenVersion FROM [User] WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql,
                new
                {
                    UserId = userId
                });
        }

        public async Task<User?> GetByEmailVerificationToken(string token)
        {
            var sql = @"SELECT * FROM [User] WHERE EmailVerificationToken = @Token";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(sql,
                new
                {
                    Token = token
                });
        }

        public async Task VerifyEmail(int userId)
        {
            var sql = @"UPDATE [User] SET IsEmailVerified =1, EmailVerificationToken = NULL, EmailVerificationExpiresAt=  NULL,UpdatedAt = GETDATE() WHERE UserId =@UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql,
                new
                {
                    UserId = userId
                });
        }


        public async Task UpdateEmailVerificationToken(int userId, string token, DateTime expiresAt)
        {
            var sql = @"UPDATE [User] SET EmailVerificationToken = @Token, EmailVerificationExpiresAt = @ExpiresAt, UpdatedAt = GETDATE() WHERE UserId = @UserId;";

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                sql,
                new
                {
                    UserId = userId,
                    Token = token,
                    ExpiresAt = expiresAt
                });
        }

        public async Task UpdateEmail(int userId, string email)
        {
            var sql = @"UPDATE [User] SET Email = @Email, UpdatedAt = GETDATE() WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                Email = email
            });
        }

        public async Task<User?> GetByEmail(string email)
        {
            var sql = @"SELECT * FROM [User] WHERE Email = @Email";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new
                {
                    Email = email
                });

        }

        public async Task UpdatePassword(int userId, string Password)
        {
            var sql = @"UPDATE [User] SET Password = @Password, UpdatedAt = GETDATE() WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                Password = Password
            });
        }

        public async Task UpdateUsername(int userId, string username)
        {
            var sql = @"UPDATE [User] SET Username = @Username , UpdatedAt = GETDATE() WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                Username = username
            });
        }

        public async Task UpdatePreferredCalendar(int userId, string preferredCalendar)
        {
            var sql = @"UPDATE [User] SET PreferredCalendar = @PreferredCalendar, UpdatedAt = GETDATE() WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                PreferredCalendar = preferredCalendar
            });
        }

        public async Task UpdatePasswordResetToken(int userId, string token, DateTime expiresAt)
        {
            var sql = @"UPDATE [User] SET PasswordResetToken = @Token, PasswordResetExpiresAt = @ExpiresAt, UpdatedAt = GETDATE() WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt
            });
        }

        public async Task<User?> GetByPasswordResetToken(string token)
        {
            var sql = @"SELECT * FROM [User] WHERE PasswordResetToken = @Token;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(sql,
                new
                {
                    Token = token
                });
        }

        public async Task ClearPasswordResetToken(int userId)
        {
            var sql = @"UPDATE [User] SET PasswordResetToken = NULL, PasswordResetExpiresAt = NULL,UpdatedAt = GETDATE() WHERE UserId = @UserId";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId
            });
        }

        public async Task UpdateFullName(int userId, string fullName)
        {
            var sql = @"UPDATE [User] SET FullName = @FullName, UpdatedAt = GETDATE() WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                FullName = fullName
            });
        }

        public async Task UpdateProfession(int userId, string? profession)
        {
            var sql = @"UPDATE [User] SET Profession = @Profession, UpdatedAt = GETDATE() WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                Profession = profession
            });
        }

        public async Task UpdatePendingEmail(int userId, string pendingEmail, string token, DateTime expiresAt)
        {
            var sql = @"UPDATE [User] SET PendingEmail = @PendingEmail, PendingEmailVerificationToken = @Token,PendingEmailVerificationExpiresAt= @ExpiresAt, UpdatedAt = GETDATE() WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                PendingEmail = pendingEmail,
                Token = token,
                ExpiresAt = expiresAt
            });
        }

        public async Task<User?> GetByPendingEmailVerificationToken(string token)
        {
            var sql = @"SELECT * FROM [User] WHERE PendingEmailVerificationToken = @Token;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(sql,
                new
                {
                    Token = token
                });
        }

        public async Task ConfirmPendingEmail(int userId)
        {
            var sql = @"UPDATE [User] SET Email = PendingEmail, PendingEmail = NULL, PendingEmailVerificationToken = NULL, PendingEmailVerificationExpiresAt = NULL, UpdatedAt = GETDATE() WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId
            });
        }

        public async Task DeactivateSelf(int userId)
        {
            var sql = @"UPDATE [User] SET IsActive = 0, DeactivatedBy = @UserId, DeactivatedAt = GETDATE(), DeactivationReason = 'User', TokenVersion = TokenVersion + 1,UpdatedAt = GETDATE() WHERE UserId = @UserId AND IsActive = 1;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId
            });
        }

        public async Task ReactivateSelf(int userId)
        {
            var sql = @"UPDATE [User] SET IsActive = 1, DeactivatedBy = NULL, DeactivatedAT = NULL, DeactivationReason = NULL, TokenVersion = TokenVersion + 1, UpdatedAt = GETDATE() WHERE UserId = @UserId AND IsActive = 0 AND DeactivationReason = 'User';";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                UserId = userId
            });
        }

        public async Task DeleteSelf(int userId)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync(
                    "DELETE FROM RefreshToken WHERE UserId = @UserId",
                    new { UserId = userId },
                    transaction);

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
