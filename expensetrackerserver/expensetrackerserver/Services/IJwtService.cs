using expensetrackerserver.Models;
namespace expensetrackerserver.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string GenerateEmailVerificationToken();
        string GenerateSecureToken();
    }
}
