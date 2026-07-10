using expensetrackerserver.DTOs;
using expensetrackerserver.Models;
namespace expensetrackerserver.Repositories
{
    public interface IDashboardRepository
    {
        Task<DashboardSummary> GetSummary(
            int userId,
            int year,
            int? month,
            int? categoryId);

        Task<IEnumerable<MonthlyExpenseSummary>> GetExpenseByMonth(int userId, int year);

        Task<IEnumerable<MonthlyCategorySummary>> GetMonthBreakdown(int userId, int year, int month, bool includeEmpty = false);
        Task<IEnumerable<CategoryExpenseSummary>> GetExpenseByCategory(int userId, int year, bool includeEmpty = false);
        Task<IEnumerable<CategoryMonthlySummary>> GetCategoryBreakdown(int userId, int year, int categoryId);
    }
}
