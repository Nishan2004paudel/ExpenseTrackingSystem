using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
using expensetrackerserver.Models;
using expensetrackerserver.Repositories;
namespace expensetrackerserver.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _repo;
        private readonly ICategoryRepository _catrepo;
        public ExpenseService(IExpenseRepository repo, ICategoryRepository catrepo)
        {
            _repo = repo;
            _catrepo = catrepo;
        }

        public async Task<ExpenseDto> Create(CreateExpenseDto dto, int userId)
        {
            var category = await _catrepo.GetById(dto.CategoryId);
            if (category == null)
            {
                throw new CategoryNotFoundException();
            }
            if (category.UserId != userId)
            {
                throw new CategoryAccessDeniedException("You are not allowed to use this category.");
            }

            if (dto.ExpenseDate.Date > DateTime.Today)
            {
                throw new InvalidExpenseDateException("Expense date cannot be in the future.");
            }

            var expense = new Expense
            {

                UserId = userId,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                Description = dto.Description
            };

            var expenseId = await _repo.Create(expense);

            return new ExpenseDto
            {
                ExpenseId = expenseId,
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Amount = expense.Amount,
                ExpenseDate = expense.ExpenseDate,
                Description = expense.Description
            };
        }

        public async Task<ExpenseDto> GetById(int expenseId, int userId)
        {
            var expense = await _repo.GetById(expenseId);
            if (expense == null)
            {
                throw new ExpenseNotFoundException();
            }

            if (expense.UserId != userId)
            {
                throw new ExpenseAccessDeniedException("You are not the owner of this expense");
            }
            return new ExpenseDto
            {
                ExpenseId = expense.ExpenseId,
                CategoryId = expense.CategoryId,
                CategoryName = expense.CategoryName,
                Amount = expense.Amount,
                ExpenseDate = expense.ExpenseDate,
                Description = expense.Description
            };

        }
        public async Task<ExpenseDto> Update(int expenseId, UpdateExpenseDto dto, int userId)
        {
            var expense = await _repo.GetById(expenseId);

            if (expense == null)
            {
                throw new ExpenseNotFoundException();
            }

            if (expense.UserId != userId)
            {
                throw new ExpenseAccessDeniedException("You are not allowed to modify this expense.");
            }

            var category = await _catrepo.GetById(dto.CategoryId);

            if (category == null)
            {
                throw new CategoryNotFoundException();
            }

            if (category.UserId != userId)
            {
                throw new CategoryAccessDeniedException("You are not allowed to use this category.");
            }

            if (dto.ExpenseDate.Date > DateTime.Today)
            {
                throw new InvalidExpenseDateException("Expense date cannot be in the future.");
            }

            var updatedExpense = new Expense
            {
                ExpenseId = expenseId,
                UserId = userId,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                Description = dto.Description
            };

            await _repo.Update(updatedExpense);

            return new ExpenseDto
            {
                ExpenseId = updatedExpense.ExpenseId,
                CategoryId = updatedExpense.CategoryId,
                CategoryName = category.CategoryName,
                Amount = updatedExpense.Amount,
                ExpenseDate = updatedExpense.ExpenseDate,
                Description = updatedExpense.Description
            };

        }
        public async Task SoftDelete(int expenseId, int userId)
        {


            var expense = await _repo.GetById(expenseId);

            if (expense == null)
            {
                throw new ExpenseNotFoundException();
            }

            if (expense.UserId != userId)
            {
                throw new ExpenseAccessDeniedException("You are not allowed to delete this expense.");
            }


            await _repo.SoftDelete(expenseId);
        }

        public async Task<IEnumerable<ExpenseDto>> GetFilteredExpenses(int userId, int? year, int? month, int? categoryId)
        {
            var expenses = await _repo.GetFilteredExpenses(userId, year, month, categoryId);

            return expenses.Select(e => new ExpenseDto
            {
                ExpenseId = e.ExpenseId,
                CategoryId = e.CategoryId,
                CategoryName = e.CategoryName,
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Description = e.Description
            }).ToList();
        }
    }
}

