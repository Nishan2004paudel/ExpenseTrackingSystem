using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class UpdateBudgetDto
    {
        public int? CategoryId { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal BudgetAmount { get; set; }
        [Required]
        public DateTime BudgetMonth { get; set; }
    }
}
