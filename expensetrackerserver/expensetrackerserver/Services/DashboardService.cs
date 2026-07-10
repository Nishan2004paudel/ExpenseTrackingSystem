using expensetrackerserver.DTOs;
using expensetrackerserver.Repositories;
namespace expensetrackerserver.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repo;
        public DashboardService(IDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<DashboardSummaryDto> GetSummary(
            int userId,
            int year,
            int? month,
            int? categoryId)
        {
            var summary = await _repo.GetSummary(userId,
                year,
                month,
                categoryId);

            decimal? remainingAmount = null;
            decimal? percentageUsed = null;

            if (summary.BudgetAmount.HasValue)
            {
                remainingAmount = summary.BudgetAmount.Value - summary.ExpenseAmount;
                if (summary.BudgetAmount.Value > 0)
                {
                    percentageUsed = Math.Round((summary.ExpenseAmount / summary.BudgetAmount.Value) * 100, 2);
                }
            }

            return new DashboardSummaryDto
            {
                BudgetAmount = summary.BudgetAmount,
                ExpenseAmount = summary.ExpenseAmount,
                RemainingAmount = remainingAmount,
                PercentageUsed = percentageUsed
            };
        }

        public async Task<IEnumerable<MonthlyExpenseSummaryDto>> GetExpenseByMonth(int userId, int year)
        {
            var summaries = await _repo.GetExpenseByMonth(userId, year);

            return summaries.Select(s =>
            {
                decimal? remainingAmount = null;
                decimal? percentageUsed = null;

                if (s.BudgetAmount.HasValue)
                {
                    remainingAmount = s.BudgetAmount.Value - s.ExpenseAmount;
                    if (s.BudgetAmount.Value > 0)
                    {
                        percentageUsed = Math.Round((s.ExpenseAmount / s.BudgetAmount.Value) * 100, 2);
                    }
                }

                return new MonthlyExpenseSummaryDto
                {
                    Year = s.Year,
                    Month = s.Month,

                    MonthName = new DateTime(year, s.Month, 1).ToString("MMMM"),

                    BudgetAmount = s.BudgetAmount,
                    ExpenseAmount = s.ExpenseAmount,

                    RemainingAmount = remainingAmount,
                    PercentageUsed = percentageUsed
                };

            }).ToList();
        }
        public async Task<IEnumerable<MonthlyCategorySummaryDto>> GetMonthBreakdown(int userId, int year, int month, bool includeEmpty = false)
        {
            var categories = await _repo.GetMonthBreakdown(userId, year, month, includeEmpty);

            return categories.Select(c =>
            {
                decimal? remainingAmount = null;
                decimal? percentageUsed = null;

                if (c.BudgetAmount.HasValue)
                {
                    remainingAmount = c.BudgetAmount.Value - c.ExpenseAmount;
                    if (c.BudgetAmount.Value > 0)
                    {
                        percentageUsed = Math.Round((c.ExpenseAmount / c.BudgetAmount.Value) * 100, 2);
                    }
                }

                return new MonthlyCategorySummaryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,

                    BudgetAmount = c.BudgetAmount,
                    ExpenseAmount = c.ExpenseAmount,

                    RemainingAmount = remainingAmount,
                    PercentageUsed = percentageUsed
                };

            }).ToList();
        }

        public async Task<IEnumerable<CategoryExpenseSummaryDto>> GetExpenseByCategory(int userId,
            int year,
            bool includeEmpty = false)
        {
            var categories = await _repo.GetExpenseByCategory(userId, year, includeEmpty);
            return categories.Select(c =>
            {
                decimal? remainingAmount = null;
                decimal? percentageUsed = null;

                if (c.BudgetAmount.HasValue)
                {
                    remainingAmount = c.BudgetAmount.Value - c.ExpenseAmount;

                    if (c.BudgetAmount.Value > 0)
                    {
                        percentageUsed = Math.Round((c.ExpenseAmount / c.BudgetAmount.Value) * 100, 2);
                    }
                }

                return new CategoryExpenseSummaryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,

                    BudgetAmount = c.BudgetAmount,
                    ExpenseAmount = c.ExpenseAmount,

                    RemainingAmount = remainingAmount,
                    PercentageUsed = percentageUsed
                };
            }).ToList();
        }

        public async Task<IEnumerable<CategoryMonthlySummaryDto>> GetCategoryBreakdown(int userId, int year, int categoryId)
        {
            var months = await _repo.GetCategoryBreakdown(userId, year, categoryId);

            return months.Select(m =>
            {
                decimal? remainingAmount = null;
                decimal? percentageUsed = null;

                if (m.BudgetAmount.HasValue)
                {
                    remainingAmount = m.BudgetAmount.Value - m.ExpenseAmount;
                    if (m.BudgetAmount.Value > 0)
                    {
                        percentageUsed = Math.Round((m.ExpenseAmount / m.BudgetAmount.Value) * 100, 2);
                    }
                }

                return new CategoryMonthlySummaryDto
                {
                    Year = m.Year,
                    Month = m.Month,
                    MonthName = new DateTime(year, m.Month, 1).ToString("MMMM"),
                    BudgetAmount = m.BudgetAmount,
                    ExpenseAmount = m.ExpenseAmount,
                    RemainingAmount = remainingAmount,
                    PercentageUsed = percentageUsed
                };
            }).ToList();
        }
    }
}
