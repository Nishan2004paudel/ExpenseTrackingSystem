using expensetrackerserver.DTOs;
using expensetrackerserver.Models;
namespace expensetrackerserver.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummary(
            int userId,
            int year,
            int? month,
            int? categoryId);

        Task<IEnumerable<MonthlyExpenseSummaryDto>> GetExpenseByMonth(int userId, int year);
        Task<IEnumerable<MonthlyCategorySummaryDto>> GetMonthBreakdown(int userId, int year, int month, bool includeEmpty = false);

        Task<IEnumerable<CategoryExpenseSummaryDto>> GetExpenseByCategory(int userId, int year, bool includeEmpty = false);
    }
}
