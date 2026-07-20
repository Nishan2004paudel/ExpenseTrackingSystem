using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class GoogleLoginDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
