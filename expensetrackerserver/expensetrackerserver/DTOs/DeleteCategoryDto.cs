using System.ComponentModel.DataAnnotations;
using expensetrackerserver.Enums;
namespace expensetrackerserver.DTOs
{
    public class DeleteCategoryDto
    {
        [Required]
        public CategoryDeleteAction Action { get; set; }
        public int? TargetCategoryId { get; set; }
        public string? NewCategoryName { get; set; }
        public BudgetConflictAction? ConflictAction { get; set; }
    }
}
