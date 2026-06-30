using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class LoginDto
    {
        [Required]
        
        public string Identifier { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
