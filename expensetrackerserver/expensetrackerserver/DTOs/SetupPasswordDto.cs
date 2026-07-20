using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class SetupPasswordDto
    {
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
