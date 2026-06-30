using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Profession { get; set; }
        [Required]
        public string PreferredCalendar { get; set; } = string.Empty;
    }
}
