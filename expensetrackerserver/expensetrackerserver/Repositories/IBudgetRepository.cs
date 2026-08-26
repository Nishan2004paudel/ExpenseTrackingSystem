using expensetrackerserver.Models;
using expensetrackerserver.DTOs;
namespace expensetrackerserver.Repositories
{
    public interface IBudgetRepository
    {
        Task<int> Create(BudgetLimit budget);
        Task<IEnumerable<BudgetWithCategory>> GetAllByUserId(int userId);
        Task<BudgetWithCategory?> GetById(int budgetId);
        Task<bool> BudgetExists(int userId, int? categoryId, DateTime budgetMonth, int? excludeBudgetId = null);
        Task Update(BudgetLimit budget);
        Task SoftDelete(int budgetId);
        Task TransferCategory(int sourceCategoryId, int targetCategoryId);
        Task SoftDeleteByCategory(int categoryId);
        Task<IEnumerable<BudgetConflictDto>> GetConflictingBudgets(int sourceCategoryId, int targetCategoryId);
        Task MergeConflictingBudgets(int sourceCategoryId, int targetCategoryId);
        Task DeleteConflictingSourceBudgets(int sourceCategoryId, int targetCategoryId);
        Task<BudgetUsageDto?> GetBudgetUsage(int userId, int categoryId, DateTime expenseDate);
    }
}
