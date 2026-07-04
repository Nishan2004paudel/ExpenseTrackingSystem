using expensetrackerserver.Models;

namespace expensetrackerserver.Repositories
{
    public interface IExpenseRepository
    {
        Task<int> Create(Expense expense);
        Task<IEnumerable<ExpenseWithCategory>> GetAllByUserId(int userId);
        Task<ExpenseWithCategory?> GetById(int expenseId);

        Task<bool> HasActiveExpenses(int categoryId);
        Task Update(Expense expense);
        Task SoftDelete(int expenseId);
    }
}
