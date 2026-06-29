namespace expensetrackerserver.Models
{
    public class BudgetLimit
    {
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public decimal BudgetAmount { get; set; }
        public DateOnly BudgetMonth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
