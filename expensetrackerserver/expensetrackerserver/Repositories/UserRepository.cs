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
            var sql = "SELECT UserId, Username, Email, FullName,Profession, PreferredCalendar,Role FROM [User]";
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
            var sql = @"INSERT INTO [User] (Username,Email,Password, FullName,Profession, PreferredCalendar,Role) VALUES ( @Username, @Email,@Password, @FullName, @Profession, @PreferredCalendar,@Role);
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
    }
}
