using System.ComponentModel.DataAnnotations;
namespace expensetrackerserver.DTOs
{
    public class ResendVerificationEmailDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
