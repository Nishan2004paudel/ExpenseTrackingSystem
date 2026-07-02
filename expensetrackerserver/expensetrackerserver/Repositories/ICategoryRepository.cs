using expensetrackerserver.DTOs;
using expensetrackerserver.Models;

namespace expensetrackerserver.Repositories
{
    public interface ICategoryRepository
    {
        Task<int> Create(Category category);
        Task<IEnumerable<Category>> GetAllByUserId(int userId);
        Task<Category?> GetById(int categoryId);
        Task<bool> CategoryExists(int userId, string categoryName);
        Task Update(Category category);
        Task SoftDelete(int categoryId);
    }
}
