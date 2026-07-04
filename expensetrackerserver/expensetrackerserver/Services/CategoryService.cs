using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
using expensetrackerserver.Models;
using expensetrackerserver.Repositories;
namespace expensetrackerserver.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IExpenseRepository _exrepo;
        public CategoryService(ICategoryRepository repo, IExpenseRepository exrepo)
        {
            _repo = repo;
            _exrepo = exrepo;
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
        public async Task SoftDelete(int categoryId, int userId)
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

            var hasExpenses = await _exrepo.HasActiveExpenses(categoryId);
            if (hasExpenses)
                throw new InvalidOperationException("Category contains expenses. Transfer or delete them first.");

            await _repo.SoftDelete(categoryId);
        }
    }
}
