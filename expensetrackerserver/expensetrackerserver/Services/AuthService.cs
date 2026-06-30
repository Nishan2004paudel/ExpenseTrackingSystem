using expensetrackerserver.DTOs;
using expensetrackerserver.Models;
using expensetrackerserver.Repositories;
namespace expensetrackerserver.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        public async Task<UserDetailDto> Register(RegisterUserDto dto)
        {
            if (await _repo.EmailExists(dto.Email))
            {
                throw new Exception("Email already exists.");
            }

            if (await _repo.UsernameExists(dto.Username))
            {
                throw new Exception("Username already exists.");
            }
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                FullName = dto.FullName,
                Profession = dto.Profession,
                PreferredCalendar = dto.PreferredCalendar
            };
            int newUserId = await _repo.Create(user);

            return new UserDetailDto
            {
                UserId = newUserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Profession = user.Profession,
                PreferredCalendar = user.PreferredCalendar
            };
        }

        public async Task<LoginResponseDto> Login(LoginDto dto)
        {
            var user = await _repo.GetByEmailOrUsername(dto.Identifier);

            if (user == null)
            {
                throw new Exception("Invalid username/email or password.");
            }

            if (user.Password != dto.Password)
            {
                throw new Exception("Invalid username/email or password.");
            }

            return new LoginResponseDto
            {
                Message = "Login successful",
                User = new UserDetailDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Profession = user.Profession,
                    PreferredCalendar = user.PreferredCalendar
                },
                Token = null
            };
        }
        public async Task<UserDetailDto?> GetUserDetail(int userId)
        {
            var user = await _repo.GetById(userId);

            if (user == null)
            {
                return null;
            }

            return new UserDetailDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Profession = user.Profession,
                PreferredCalendar = user.PreferredCalendar
            };
        }
    }
}
