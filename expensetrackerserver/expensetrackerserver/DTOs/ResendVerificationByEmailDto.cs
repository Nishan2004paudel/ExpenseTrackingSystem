using System.ComponentModel.DataAnnotations;
namespace expensetrackerserver.DTOs
{
    public class ResendVerificationByEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
