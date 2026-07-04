using expensetrackerserver.DTOs;
namespace expensetrackerserver.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> Create(CreateCategoryDto dto, int userId);
        Task<IEnumerable<CategoryDto>> GetAllByUserId(int userId);
        Task<CategoryDto> Update(int categoryId, UpdateCategoryDto dto, int userId);
        Task SoftDelete(int categoryId, int userId);
    }
}
