using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
using expensetrackerserver.Repositories;

namespace expensetrackerserver.Services
{
    public class SettingService : ISettingService
    {
        private readonly IUserRepository _repo;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshRepo;

        public SettingService(IUserRepository repo, IEmailService emailService, IJwtService jwtService, IRefreshTokenRepository refreshRepo)
        {
            _repo = repo;
            _emailService = emailService;
            _jwtService = jwtService;
            _refreshRepo = refreshRepo;
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

        public async Task<MessageResponseDto> ChangeEmail(int userId, ChangeEmailDto dto)
        {
            var user = await _repo.GetById(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            if (user.AuthProvider == "Google")
            {
                throw new InvalidOperationException("Google accounts cannot change email.");
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password))
            {
                throw new InvalidCredentialsException("Incorrect password.");
            }
            dto.NewEmail = dto.NewEmail.Trim();

            if (user.Email.Equals(dto.NewEmail, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("New email must be different from the current email.");
            }

            if (await _repo.EmailExists(dto.NewEmail))
            {
                throw new EmailAlreadyExistsException();
            }
            var token = _jwtService.GenerateEmailVerificationToken();
            var expiresAt = DateTime.UtcNow.AddHours(24);

            await _repo.UpdatePendingEmail(user.UserId, dto.NewEmail, token, expiresAt);

            await _emailService.SendChangeEmailVerificationAsync(dto.NewEmail, user.FullName, token);
            return new MessageResponseDto
            {
                Message = "Verification email sent to your new email address."
            };
        }

        public async Task<MessageResponseDto> VerifyEmailChange(string token)
        {
            var user = await _repo.GetByPendingEmailVerificationToken(token);
            if (user == null)
            {
                throw new InvalidEmailVerificationException("Invalid verification link.");
            }

            if (user.PendingEmailVerificationExpiresAt == null || user.PendingEmailVerificationExpiresAt < DateTime.UtcNow)
            {
                throw new VerificationLinkExpiredException("Verification link has expired.");
            }

            await _repo.ConfirmPendingEmail(user.UserId);

            await _repo.IncrementTokenVersion(user.UserId);
            await _refreshRepo.RevokeAllByUserId(user.UserId);
            return new MessageResponseDto
            {
                Message = "Email Changed Successfully."
            };
        }

        public async Task<MessageResponseDto> ChangePassword(int userId, ChangePasswordDto dto)
        {
            var user = await _repo.GetById(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                throw new InvalidOperationException("Please set a password first.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password))
            {
                throw new InvalidCredentialsException("Current password is incorrect.");
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                throw new InvalidPasswordException("Passwords do not match.");
            }

            ValidatePassword(dto.NewPassword);

            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.Password))
            {
                throw new InvalidPasswordException("New password cannot be the same as your current password.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _repo.UpdatePassword(user.UserId, hashedPassword);
            await _repo.IncrementTokenVersion(user.UserId);
            await _refreshRepo.RevokeAllByUserId(user.UserId);
            return new MessageResponseDto
            {
                Message = "Password changed successfully. Please log in again."
            };
        }

        public async Task<MessageResponseDto> ChangeUsername(int userId, ChangeUsernameDto dto)
        {
            var user = await _repo.GetById(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                throw new InvalidOperationException("Please set a password first.");
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password))
            {
                throw new InvalidCredentialsException("Current password is incorrect.");
            }
            dto.NewUsername = dto.NewUsername.Trim();
            if (string.IsNullOrWhiteSpace(dto.NewUsername))
            {
                throw new InvalidOperationException("Username is invalid.");
            }
            if (string.Equals(user.Username, dto.NewUsername, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("New username must be different from the current username.");
            }
            if (await _repo.UsernameExists(dto.NewUsername))
            {
                throw new UsernameAlreadyExistsException();
            }
            await _repo.UpdateUsername(user.UserId, dto.NewUsername);
            return new MessageResponseDto
            {
                Message = "Username changed successfully."
            };

        }
    }
}