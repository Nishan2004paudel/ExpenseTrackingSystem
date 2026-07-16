using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
using expensetrackerserver.Models;
using expensetrackerserver.Repositories;
using System.Security.Cryptography;
namespace expensetrackerserver.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository repo, IJwtService jwtService, IRefreshTokenRepository refreshRepo, IEmailService emailService)
        {
            _repo = repo;
            _jwtService = jwtService;
            _refreshRepo = refreshRepo;
            _emailService = emailService;
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

            var verificationToken = _jwtService.GenerateEmailVerificationToken();
            var expiresAt = DateTime.UtcNow.AddHours(24);
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Profession = dto.Profession,
                PreferredCalendar = dto.PreferredCalendar,
                Role = "User",
                IsEmailVerified = false,
                EmailVerificationToken = verificationToken,
                EmailVerificationExpiresAt = expiresAt
            };
            int newUserId = await _repo.Create(user);
            await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, verificationToken);

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

            if (!user.IsEmailVerified)
            {
                throw new EmailNotVerifiedException("Please verify your email before logging in.");
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

        public async Task VerifyEmail(string token)
        {
            var user = await _repo.GetByEmailVerificationToken(token);
            if (user == null)
            {
                throw new InvalidEmailVerificationException("Invalid verification token.");
            }
            if (user.IsEmailVerified)
            {
                throw new EmailAlreadyVerifiedException("Email already Verified.");
            }
            if (user.EmailVerificationExpiresAt == null || user.EmailVerificationExpiresAt < DateTime.UtcNow)
            {
                throw new VerificationLinkExpiredException("Verification Link Expired.");
            }
            await _repo.VerifyEmail(user.UserId);
        }

        public async Task ResendVerificationEmail(ResendVerificationEmailDto dto)
        {
            var user = await _repo.GetById(dto.UserId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            if (user.IsEmailVerified)
            {
                throw new EmailAlreadyVerifiedException("Email already Verified.");
            }
            if (!user.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _repo.EmailExists(dto.Email))
                {
                    throw new EmailAlreadyExistsException();
                }

                await _repo.UpdateEmail(user.UserId, dto.Email);
                user.Email = dto.Email;
            }

            var verificationToken = _jwtService.GenerateEmailVerificationToken();
            var expiresAt = DateTime.UtcNow.AddHours(24);

            await _repo.UpdateEmailVerificationToken(user.UserId, verificationToken, expiresAt);
            await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, verificationToken);
        }

        public async Task ResendVerificationByEmail(ResendVerificationByEmailDto dto)
        {
            var user = await _repo.GetByEmail(dto.Email);
            if (user == null)
            {
                return;
            }
            if (user.IsEmailVerified)
            {
                return;
            }

            var verificationToken = _jwtService.GenerateEmailVerificationToken();
            var expiresAt = DateTime.UtcNow.AddHours(24);

            await _repo.UpdateEmailVerificationToken(user.UserId, verificationToken, expiresAt);
            await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, verificationToken);
        }
    }
}
