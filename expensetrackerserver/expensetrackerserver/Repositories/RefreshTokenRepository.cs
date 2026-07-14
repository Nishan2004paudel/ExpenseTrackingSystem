using Dapper;
using expensetrackerserver.Data;
using expensetrackerserver.Models;

namespace expensetrackerserver.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DapperContext _context;
        public RefreshTokenRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> Create(RefreshToken refreshToken)
        {
            var sql = @"INSERT INTO RefreshToken (UserId, Token, ExpiresAt) VALUES (@UserId, @Token,@ExpiresAt); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                sql,
                refreshToken);
        }
        public async Task<RefreshToken?> GetActiveByToken(string token)
        {
            var sql = @"SELECT * FROM RefreshToken WHERE Token = @Token AND RevokedAt IS NULL AND ExpiresAt > GETDATE();";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<RefreshToken>(
                sql,
                new
                {
                    Token = token
                });
        }
        public async Task Revoke(int refreshTokenId)
        {
            var sql = @"UPDATE RefreshToken SET RevokedAt = GETDATE() WHERE RefreshTokenId = @RefreshTokenId AND RevokedAt IS NULL;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                sql,
                new
                {
                    RefreshTokenId = refreshTokenId
                });
        }

        public async Task RevokeAllByUserId(int userId)
        {
            var sql = @"UPDATE RefreshToken SET RevokedAt = GETDATE() WHERE UserId = @UserId AND RevokedAt IS NULL;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                sql,
                new
                {
                    UserId = userId
                });
        }

    }
}
