using expensetrackerserver.Models;

namespace expensetrackerserver.Repositories
{
    public interface IExpenseRepository
    {
        Task<int> Create(Expense expense);
        Task<ExpenseWithCategory?> GetById(int expenseId);

        Task<bool> HasActiveExpenses(int categoryId);
        Task Update(Expense expense);
        Task SoftDelete(int expenseId);
        Task<IEnumerable<ExpenseWithCategory>> GetFilteredExpenses(int userId, int? year, int? month, int? categoryId);

        Task TransferCategory(int sourceCategoryId, int targetCategoryId);
        Task SoftDeleteByCategory(int categoryId);

    }
}
