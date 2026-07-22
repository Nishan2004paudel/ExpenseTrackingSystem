using System.ComponentModel.DataAnnotations;
namespace expensetrackerserver.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]

        public string Identifier { get; set; } = string.Empty;
    }
}
