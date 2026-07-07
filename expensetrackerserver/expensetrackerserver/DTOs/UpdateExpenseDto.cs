using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    //client to server
    public class UpdateExpenseDto
    {
        [Range(1,int.MaxValue,ErrorMessage ="Please select a valid category.")]
        public int CategoryId { get; set; }
        [Range(0.01,double.MaxValue,ErrorMessage ="Amount must be greater than zero.")]
        public decimal Amount { get; set; }
        [Required]
        public DateTime ExpenseDate { get; set; }
        [StringLength(255)]
        public string? Description { get; set; }
    }
}
