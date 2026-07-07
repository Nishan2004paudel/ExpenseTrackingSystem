namespace expensetrackerserver.DTOs
{
    public class DashboardSummaryDto
    {
        public decimal? BudgetAmount { get; set; }
        public decimal ExpenseAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
        public decimal? PercentageUsed { get; set; }
    }
}
