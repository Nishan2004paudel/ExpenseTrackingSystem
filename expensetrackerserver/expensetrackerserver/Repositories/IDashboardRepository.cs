using expensetrackerserver.DTOs;
namespace expensetrackerserver.Repositories
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDto> GetMonthlySummary(
            int userId,
            DateTime month,
            int? categoryId);
    }
}
