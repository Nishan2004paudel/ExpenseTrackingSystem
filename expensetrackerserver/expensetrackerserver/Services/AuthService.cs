using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
using expensetrackerserver.Models;
using expensetrackerserver.Repositories;
namespace expensetrackerserver.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshRepo;

        public AuthService(IUserRepository repo, IJwtService jwtService, IRefreshTokenRepository refreshRepo)
        {
            _repo = repo;
            _jwtService = jwtService;
            _refreshRepo = refreshRepo;
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
                throw new InvalidCredentialsException("Invalid username/email or password.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                throw new InvalidCredentialsException("Invalid username/email or password.");
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshRepo.Create(refreshTokenEntity);

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
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        public async Task<UserDetailDto> GetUserDetail(int userId)
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

        public async Task<RefreshTokenResponseDto> Refresh(string refreshToken)
        {
            var storedToken = await _refreshRepo.GetActiveByToken(refreshToken);

            if (storedToken == null)
            {
                throw new InvalidRefreshTokenException("Invalid refresh token.");
            }
            if (storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new InvalidRefreshTokenException("Refresh token expired.");
            }
            var user = await _repo.GetById(storedToken.UserId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            await _refreshRepo.Revoke(storedToken.RefreshTokenId);

            var accessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            await _refreshRepo.Create(new RefreshToken
            {
                UserId = user.UserId,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            return new RefreshTokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task Logout(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return;
            }
            var storedToken = await _refreshRepo.GetActiveByToken(refreshToken);

            if (storedToken == null)
            {
                return;
            }

            await _refreshRepo.Revoke(storedToken.RefreshTokenId);
        }

        public async Task LogoutEverywhere(int userId)
        {
            await _repo.IncrementTokenVersion(userId);
            await _refreshRepo.RevokeAllByUserId(userId);
        }
    }
}
