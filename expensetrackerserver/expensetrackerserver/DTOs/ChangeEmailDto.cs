using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class ChangeEmailDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        [EmailAddress]
        public string NewEmail { get; set; } = string.Empty;
    }
}
