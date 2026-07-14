using expensetrackerserver.Models;
namespace expensetrackerserver.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<int> Create(RefreshToken refreshToken);
        Task<RefreshToken?> GetActiveByToken(string token);
        Task Revoke(int refreshTokenId);
        Task RevokeAllByUserId(int userId);
    }
}
