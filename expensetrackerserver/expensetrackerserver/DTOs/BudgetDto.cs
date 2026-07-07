namespace expensetrackerserver.DTOs
{
    public class BudgetDto
    {
        public int BudgetId { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal BudgetAmount { get; set; }
        public DateTime BudgetMonth { get; set; }
    }
}
