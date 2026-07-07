using expensetrackerserver.DTOs;

namespace expensetrackerserver.Services
{
    public interface IBudgetService
    {
        Task<BudgetDto> Create(CreateBudgetDto dto, int userId);
        Task<IEnumerable<BudgetDto>> GetAllByUserId(int userId);
        Task<BudgetDto> GetById(int budgetId, int userId);
        Task<BudgetDto> Update(int budgetId, UpdateBudgetDto dto, int userId);
        Task SoftDelete(int budgetId, int userId);
    }
}
