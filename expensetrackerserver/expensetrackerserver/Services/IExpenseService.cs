using expensetrackerserver.DTOs;

namespace expensetrackerserver.Services
{
    public interface IExpenseService
    {
        Task<ExpenseDto> Create(CreateExpenseDto dto, int userId);
        Task<ExpenseDto> GetById(int expenseId, int userId);
        Task<ExpenseDto> Update(int expenseId, UpdateExpenseDto dto, int userId);
        Task SoftDelete(int expenseId, int userId);
        Task<IEnumerable<ExpenseDto>> GetFilteredExpenses(int userId, int? year, int? month, int? categoryId);
    }
}
