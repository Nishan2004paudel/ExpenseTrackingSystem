namespace expensetrackerserver.Models
{
    public class CategoryMonthlySummary
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal? BudgetAmount { get; set; }
        public decimal ExpenseAmount { get; set; }
    }
}
