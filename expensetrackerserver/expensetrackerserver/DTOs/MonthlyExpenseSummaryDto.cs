namespace expensetrackerserver.DTOs
{
    public class MonthlyExpenseSummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal? BudgetAmount { get; set; }
        public decimal ExpenseAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
        public decimal? PercentageUsed { get; set; }
    }
}
