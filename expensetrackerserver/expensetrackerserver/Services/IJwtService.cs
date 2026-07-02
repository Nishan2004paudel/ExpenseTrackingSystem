using expensetrackerserver.Models;
namespace expensetrackerserver.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
