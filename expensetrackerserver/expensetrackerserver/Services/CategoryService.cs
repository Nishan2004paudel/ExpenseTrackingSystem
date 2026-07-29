using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
using expensetrackerserver.Models;
using expensetrackerserver.Repositories;
using expensetrackerserver.Enums;
namespace expensetrackerserver.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IExpenseRepository _exrepo;
        private readonly IBudgetRepository _budgetrepo;
        public CategoryService(ICategoryRepository repo, IExpenseRepository exrepo, IBudgetRepository budgetrepo)
        {
            _repo = repo;
            _exrepo = exrepo;
            _budgetrepo = budgetrepo;
        }

        public async Task<CategoryDto> Create(CreateCategoryDto dto, int userId)
        {
            if (await _repo.CategoryExists(userId, dto.CategoryName))
            {
                throw new CategoryAlreadyExistsException();
            }

            var category = new Category
            {
                UserId = userId,
                CategoryName = dto.CategoryName
            };
            //we transformed category dto to models as repository works with models

            var categoryId = await _repo.Create(category);

            return new CategoryDto
            {
                CategoryId = categoryId,
                CategoryName = category.CategoryName
            };
        }
        public async Task<IEnumerable<CategoryDto>> GetAllByUserId(int userId)
        {
            var categories = await _repo.GetAllByUserId(userId);

            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            })
                .ToList();
        }
        public async Task<CategoryDto> Update(int categoryId, UpdateCategoryDto dto, int userId)
        {
            var category = await _repo.GetById(categoryId);

            if (category == null)
            {
                throw new CategoryNotFoundException();
            }

            if (category.UserId != userId)
            {
                throw new CategoryAccessDeniedException("You are not allowed to modify this category.");
            }

            if (await _repo.CategoryExists(userId, dto.CategoryName, categoryId))
            {
                throw new CategoryAlreadyExistsException();
            }

            category.CategoryName = dto.CategoryName;
            await _repo.Update(category);

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }
        public async Task<CategoryDeleteConflictDto?> Delete(int categoryId, DeleteCategoryDto dto, int userId)
        {
            var category = await _repo.GetById(categoryId);

            if (category == null)
            {
                throw new CategoryNotFoundException();
            }

            if (category.UserId != userId)
            {
                throw new CategoryAccessDeniedException("You are not allowed to delete this category.");
            }

            if (dto.Action == CategoryDeleteAction.DeleteAll)
            {
                await _exrepo.SoftDeleteByCategory(categoryId);
                await _budgetrepo.SoftDeleteByCategory(categoryId);
                await _repo.SoftDelete(categoryId);

                return null;
            }

            if (dto.Action == CategoryDeleteAction.TransferToExisting)
            {
                if (!dto.TargetCategoryId.HasValue)
                {
                    throw new InvalidOperationException("Target Category is required");
                }
                if (dto.TargetCategoryId.Value == categoryId)
                {
                    throw new InvalidOperationException("Cannot transfer to the same category");
                }

                var targetCategory = await _repo.GetById(dto.TargetCategoryId.Value);
                if (targetCategory == null)
                {
                    throw new CategoryNotFoundException();
                }
                if (targetCategory.UserId != userId)
                {
                    throw new CategoryAccessDeniedException("You are not allowed to use this category.");
                }

                var conflicts = (await _budgetrepo.GetConflictingBudgets(categoryId, targetCategory.CategoryId)).ToList();
                if (conflicts.Any() && dto.ConflictAction == null)
                {
                    return new CategoryDeleteConflictDto
                    {

                        Conflicts = conflicts
                    };
                }
                if (conflicts.Any())
                {
                    switch (dto.ConflictAction)
                    {
                        case BudgetConflictAction.Merge:
                            await _budgetrepo.MergeConflictingBudgets(categoryId, targetCategory.CategoryId);
                            break;

                        case BudgetConflictAction.DeleteSource:
                            await _budgetrepo.DeleteConflictingSourceBudgets(categoryId, targetCategory.CategoryId);
                            break;

                        default:
                            throw new ArgumentException("Invalid conflict action.");
                    }
                }
                await _budgetrepo.TransferCategory(categoryId, targetCategory.CategoryId);
                await _exrepo.TransferCategory(categoryId, targetCategory.CategoryId);
                await _repo.SoftDelete(categoryId);
                return null;
            }

            if (dto.Action == CategoryDeleteAction.TransferToNew)
            {
                if (string.IsNullOrWhiteSpace(dto.NewCategoryName))
                {
                    throw new ArgumentException("New category name is required.");
                }
                var categoryName = dto.NewCategoryName.Trim();
                if (await _repo.CategoryExists(userId, categoryName))
                {
                    throw new CategoryAlreadyExistsException();
                }

                var newCategory = new Category
                {
                    UserId = userId,
                    CategoryName = categoryName
                };

                var newCategoryId = await _repo.Create(newCategory);
                await _exrepo.TransferCategory(categoryId, newCategoryId);
                await _budgetrepo.TransferCategory(categoryId, newCategoryId);
                await _repo.SoftDelete(categoryId);

                return null;
            }

            throw new ArgumentException("Invalid delete action.");
        }
    }
}
