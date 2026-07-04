using Dapper;
using expensetrackerserver.Data;
using expensetrackerserver.Models;

namespace expensetrackerserver.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DapperContext _context;
        public CategoryRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> Create(Category category)
        {
            var sql = @"INSERT INTO Category (UserId, CategoryName) VALUES (@UserId, @CategoryName);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, category);
        }
        public async Task<IEnumerable<Category>> GetAllByUserId(int userId)
        {
            var sql = @"SELECT * FROM Category WHERE UserId = @UserId
                        AND IsDeleted = 0 ORDER BY CreatedAt DESC;";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Category>(
                sql,
                new { UserId = userId });
        }
        public async Task<Category?> GetById(int categoryId)
        {
            var sql = @"SELECT * FROM Category WHERE CategoryId = @CategoryId
                            AND IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Category>(
                sql,
                new { CategoryId = categoryId });
        }
        public async Task<bool> CategoryExists(int userId, string categoryName, int? excludeCategoryId = null)
        {
            var sql = @"SELECT COUNT(1) FROM Category WHERE UserId = @UserId AND CategoryName = @CategoryName AND IsDeleted = 0 AND (@ExcludeCategoryId IS NULL OR CategoryId <> @ExcludeCategoryId);";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    UserId = userId,
                    CategoryName = categoryName,
                    ExcludeCategoryId = excludeCategoryId
                });
            return count > 0;
        }
        public async Task Update(Category category)
        {
            var sql = @"UPDATE Category SET CategoryName = @CategoryName, UpdatedAt = GETDATE() WHERE CategoryId = @CategoryId AND IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, category);
        }
        public async Task SoftDelete(int categoryId)
        {
            var sql = @"UPDATE Category SET IsDeleted =1, UpdatedAt = GETDATE() WHERE CategoryId = @CategoryId AND IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                sql,
                new { CategoryId = categoryId });
        }
    }
}
