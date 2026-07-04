namespace expensetrackerserver.Models
{
    public class ExpenseWithCategory
    {
        public int ExpenseId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateOnly ExpenseDate { get; set; }
        public string? Description { get; set; }
    }
}
