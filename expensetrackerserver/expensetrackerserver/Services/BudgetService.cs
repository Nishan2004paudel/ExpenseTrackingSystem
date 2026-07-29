using expensetrackerserver.Repositories;
using expensetrackerserver.Models;
using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace expensetrackerserver.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _repo;
        private readonly ICategoryRepository _carepo;
        public BudgetService(IBudgetRepository repo, ICategoryRepository carepo)
        {
            _repo = repo;
            _carepo = carepo;
        }

        private void ValidateBudgetMonth(DateTime budgetMonth)
        {
            var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var selectedMonth = new DateTime(budgetMonth.Year, budgetMonth.Month, 1);
            if (selectedMonth < currentMonth)
            {
                throw new PastBudgetMonthNotAllowedException("You cannot create or update budgets for past months.");
            }
        }

        public async Task<BudgetDto> Create(CreateBudgetDto dto, int userId)
        {
            Category? category = null;
            //validate category only if this is a category wise budget
            if (dto.CategoryId.HasValue)
            {
                category = await _carepo.GetById(dto.CategoryId.Value);

                if (category == null)
                {
                    throw new CategoryNotFoundException();
                }

                if (category.UserId != userId)
                {
                    throw new CategoryAccessDeniedException("You are not allowed to use this category.");
                }
            }
            var budgetMonth = new DateTime(
    dto.BudgetMonth.Year,
    dto.BudgetMonth.Month,
    1);
            ValidateBudgetMonth(budgetMonth);
            var budgetExists = await _repo.BudgetExists(userId, dto.CategoryId, budgetMonth);

            if (budgetExists)
            {
                throw new BudgetAlreadyExistsException();
            }

            var budget = new BudgetLimit
            {
                UserId = userId,
                CategoryId = dto.CategoryId,
                BudgetAmount = dto.BudgetAmount,
                BudgetMonth = budgetMonth
            };

            var budgetId = await _repo.Create(budget);

            return new BudgetDto
            {
                BudgetId = budgetId,
                CategoryId = budget.CategoryId,
                CategoryName = category?.CategoryName,
                BudgetAmount = budget.BudgetAmount,
                BudgetMonth = budget.BudgetMonth
            };
        }
        public async Task<IEnumerable<BudgetDto>> GetAllByUserId(int userId)
        {
            var budgets = await _repo.GetAllByUserId(userId);

            return budgets.Select(b => new BudgetDto
            {
                BudgetId = b.BudgetId,
                CategoryId = b.CategoryId,
                CategoryName = b.CategoryName,
                BudgetAmount = b.BudgetAmount,
                BudgetMonth = b.BudgetMonth
            })
                .ToList();
        }
        public async Task<BudgetDto> GetById(int budgetId, int userId)
        {
            var budget = await _repo.GetById(budgetId);
            if (budget == null)
            {
                throw new BudgetNotFoundException();
            }

            if (budget.UserId != userId)
            {
                throw new BudgetAccessDeniedException("You are not the owner of this budget");
            }
            return new BudgetDto
            {
                BudgetId = budget.BudgetId,
                CategoryId = budget.CategoryId,
                CategoryName = budget.CategoryName,
                BudgetAmount = budget.BudgetAmount,
                BudgetMonth = budget.BudgetMonth
            };

        }
        public async Task<BudgetDto> Update(int budgetId, UpdateBudgetDto dto, int userId)
        {
            var budget = await _repo.GetById(budgetId);

            if (budget == null)
            {
                throw new BudgetNotFoundException();
            }

            if (budget.UserId != userId)
            {
                throw new BudgetAccessDeniedException("You are not allowed to modify this budget.");
            }
            Category? category = null;

            if (dto.CategoryId.HasValue)
            {
                category = await _carepo.GetById(dto.CategoryId.Value);

                if (category == null)
                {
                    throw new CategoryNotFoundException();
                }

                if (category.UserId != userId)
                {
                    throw new CategoryAccessDeniedException("You are not allowed to use this category");
                }
            }

            var budgetMonth = new DateTime(
dto.BudgetMonth.Year,
dto.BudgetMonth.Month,
1);
            ValidateBudgetMonth(budgetMonth);
            if (await _repo.BudgetExists(userId, dto.CategoryId, budgetMonth, budgetId))
            {
                throw new BudgetAlreadyExistsException();
            }

            var updatedBudget = new BudgetLimit
            {
                BudgetId = budgetId,
                UserId = userId,
                CategoryId = dto.CategoryId,
                BudgetAmount = dto.BudgetAmount,
                BudgetMonth = budgetMonth
            };

            await _repo.Update(updatedBudget);

            return new BudgetDto
            {
                BudgetId = updatedBudget.BudgetId,
                CategoryId = updatedBudget.CategoryId,
                CategoryName = category?.CategoryName,
                BudgetAmount = updatedBudget.BudgetAmount,
                BudgetMonth = updatedBudget.BudgetMonth
            };
        }
        public async Task SoftDelete(int budgetId, int userId)
        {
            var budget = await _repo.GetById(budgetId);

            if (budget == null)
            {
                throw new BudgetNotFoundException();
            }

            if (budget.UserId != userId)
            {
                throw new BudgetAccessDeniedException("You are not allowed to delete this budget.");
            }


            await _repo.SoftDelete(budgetId);
        }
    }
}
