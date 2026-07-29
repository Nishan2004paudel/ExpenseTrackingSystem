public class BudgetConflictDto
{
    public DateTime BudgetMonth { get; set; }

    public int SourceCategoryId { get; set; }
    public string SourceCategoryName { get; set; } = string.Empty;

    public int TargetCategoryId { get; set; }
    public string TargetCategoryName { get; set; } = string.Empty;

    public decimal SourceBudgetAmount { get; set; }
    public decimal TargetBudgetAmount { get; set; }
}