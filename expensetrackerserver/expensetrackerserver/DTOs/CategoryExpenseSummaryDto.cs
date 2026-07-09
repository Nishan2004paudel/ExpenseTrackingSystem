namespace expensetrackerserver.DTOs
{
    public class CategoryExpenseSummaryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal? BudgetAmount { get; set; }
        public decimal ExpenseAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
        public decimal? PercentageUsed { get; set; }
    }
}
