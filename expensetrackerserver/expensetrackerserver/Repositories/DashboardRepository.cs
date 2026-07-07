using expensetrackerserver.Data;
using expensetrackerserver.DTOs;

namespace expensetrackerserver.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DapperContext _context;
        public DashboardRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetMonthlySummary(
            int userId,
            DateTime month,
            int? categoryId)
        {
            throw new NotImplementedException();
        }
    }
}
