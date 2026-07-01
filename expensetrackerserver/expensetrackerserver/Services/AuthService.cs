using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
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
            ValidatePassword(dto.Password);
            ValidatePreferredCalendar(dto.PreferredCalendar);
            if (await _repo.EmailExists(dto.Email))
            {
                throw new EmailAlreadyExistsException();
            }

            if (await _repo.UsernameExists(dto.Username))
            {
                throw new UsernameAlreadyExistsException();
            }
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Profession = dto.Profession,
                PreferredCalendar = dto.PreferredCalendar,
                Role = "User"
            };
            int newUserId = await _repo.Create(user);

            return new UserDetailDto
            {
                UserId = newUserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Profession = user.Profession,
                PreferredCalendar = user.PreferredCalendar,
                Role = user.Role
            };
        }

        private void ValidatePassword(string password)
        {
            if (!password.Any(char.IsUpper))
            {
                throw new InvalidPasswordException("Password must contain at least one uppercase letter.");
            }

            if (!password.Any(char.IsLower))
            {
                throw new InvalidPasswordException("Password must contain at least one lowercase letter.");
            }

            if (!password.Any(char.IsDigit))
            {
                throw new InvalidPasswordException("Password must contain at least one number.");
            }
        }

        private void ValidatePreferredCalendar(string preferredCalendar)
        {
            if (!preferredCalendar.Equals("English", StringComparison.OrdinalIgnoreCase) && !preferredCalendar.Equals("Nepali", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidPreferredCalendarException();
            }
        }

        public async Task<LoginResponseDto> Login(LoginDto dto)
        {
            var user = await _repo.GetByEmailOrUsername(dto.Identifier);

            if (user == null)
            {
                throw new InvalidCredentialsException();
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                throw new InvalidCredentialsException();
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
                    PreferredCalendar = user.PreferredCalendar,
                    Role = user.Role
                },
                Token = null
            };
        }
        public async Task<UserDetailDto?> GetUserDetail(int userId)
        {
            var user = await _repo.GetById(userId);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            return new UserDetailDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Profession = user.Profession,
                PreferredCalendar = user.PreferredCalendar,
                Role = user.Role
            };
        }
    }
}
