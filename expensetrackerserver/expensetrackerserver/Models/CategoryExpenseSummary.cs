namespace expensetrackerserver.Models
{
    public class CategoryExpenseSummary
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal? BudgetAmount { get; set; }
        public decimal ExpenseAmount { get; set; }
    }
}
