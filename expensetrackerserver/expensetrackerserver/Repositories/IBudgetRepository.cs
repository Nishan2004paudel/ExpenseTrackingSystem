using expensetrackerserver.Models;

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
    }
}
