using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    //client to server
    public class CreateCategoryDto
    {
        [Required]
        public string CategoryName { get; set; } = string.Empty;
    }
}
