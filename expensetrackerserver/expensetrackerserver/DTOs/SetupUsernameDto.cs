using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class SetupUsernameDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
    }
}
