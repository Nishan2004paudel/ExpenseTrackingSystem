using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        [StringLength(100)]
        public string? Profession { get; set; }
        [Required]
        public string PreferredCalendar { get; set; } = string.Empty;
    }
}
