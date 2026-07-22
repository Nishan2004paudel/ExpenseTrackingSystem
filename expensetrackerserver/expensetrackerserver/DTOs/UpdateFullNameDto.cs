using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class UpdateFullNameDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}
