using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
using expensetrackerserver.Repositories;

namespace expensetrackerserver.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _repo;

        public ProfileService(IUserRepository repo)
        {
            _repo = repo;
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

        public async Task<MessageResponseDto> SetupPassword(int userId, SetupPasswordDto dto)
        {
            var user = await _repo.GetById(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            if (!string.IsNullOrEmpty(user.Password))
            {
                throw new PasswordAlreadySetException();
            }

            ValidatePassword(dto.Password);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            await _repo.UpdatePassword(user.UserId, hashedPassword);
            return new MessageResponseDto
            {
                Message = "Password has been set successfully."
            };
        }

        public async Task<MessageResponseDto> SetupUsername(int userId, SetupUsernameDto dto)
        {
            var user = await _repo.GetById(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            if (!string.IsNullOrEmpty(user.Username))
            {
                throw new UsernameAlreadySetException();
            }
            dto.Username = dto.Username.Trim();
            if (await _repo.UsernameExists(dto.Username))
            {
                throw new UsernameAlreadyExistsException();
            }

            await _repo.UpdateUsername(user.UserId, dto.Username);
            return new MessageResponseDto
            {
                Message = "Username set successfully."
            };
        }

        public async Task<MessageResponseDto> ChangePreferredCalendar(int userId, UpdatePreferredCalendarDto dto)
        {
            var user = await _repo.GetById(userId);

            if (user == null)
            {
                throw new UserNotFoundException();
            }
            dto.PreferredCalendar = dto.PreferredCalendar.Trim();
            ValidatePreferredCalendar(dto.PreferredCalendar);

            await _repo.UpdatePreferredCalendar(user.UserId, dto.PreferredCalendar);

            return new MessageResponseDto
            {
                Message = "Preferred calendar updated successfully"
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
                Role = user.Role,
                AuthProvider = user.AuthProvider,
                HasPassword = !string.IsNullOrEmpty(user.Password)
            };
        }

        public async Task<MessageResponseDto> UpdateFullName(int userId, UpdateFullNameDto dto)
        {
            var user = await _repo.GetById(userId);

            if (user == null)
            {
                throw new UserNotFoundException();
            }


            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new InvalidFullNameException("Full name cannot be empty.");
            }
            dto.FullName = dto.FullName.Trim();
            await _repo.UpdateFullName(user.UserId, dto.FullName);

            return new MessageResponseDto
            {
                Message = "Full name updated successfully."
            };
        }

        public async Task<MessageResponseDto> UpdateProfession(int userId, UpdateProfessionDto dto)
        {
            var user = await _repo.GetById(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            dto.Profession = string.IsNullOrWhiteSpace(dto.Profession)
                ? null
                : dto.Profession.Trim();

            await _repo.UpdateProfession(user.UserId, dto.Profession);

            return new MessageResponseDto
            {
                Message = "Profession updated successfully."
            };
        }

    }
}
