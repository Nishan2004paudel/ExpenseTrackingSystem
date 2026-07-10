using Dapper;
using expensetrackerserver.Data;
using expensetrackerserver.Models;

namespace expensetrackerserver.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly DapperContext _context;
        public BudgetRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> Create(BudgetLimit budget)
        {
            var sql = @"INSERT INTO BudgetLimit (UserId, CategoryId, BudgetAmount, BudgetMonth) VALUES (@UserId, @CategoryId ,@BudgetAmount, @BudgetMonth); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, budget);
        }
        public async Task<IEnumerable<BudgetWithCategory>> GetAllByUserId(int userId)
        {
            var sql = @"SELECT b.BudgetId ,b.UserId, b.CategoryId, c.CategoryName,b.BudgetAmount,b.BudgetMonth FROM BudgetLimit b LEFT JOIN Category c ON b.CategoryId = c.CategoryId
                            WHERE b.UserId = @UserId AND b.IsDeleted =0 ORDER BY b.BudgetMonth DESC ,b.CreatedAt DESC;";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<BudgetWithCategory>(
                sql,
                new { UserId = userId });
        }
        public async Task<BudgetWithCategory?> GetById(int budgetId)
        {
            var sql = @"SELECT b.BudgetId, b.UserId, b.CategoryId, c.CategoryName,b.BudgetAmount,b.BudgetMonth FROM BudgetLimit b LEFT JOIN Category c ON b.CategoryId = c.CategoryId WHERE b.BudgetId = @BudgetId AND b.IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<BudgetWithCategory>(
                sql,
                new { BudgetId = budgetId });
        }
        public async Task<bool> BudgetExists(int userId, int? categoryId, DateTime budgetMonth, int? excludeBudgetId = null)
        {
            var sql = @"SELECT COUNT(1) FROM BudgetLimit WHERE UserId = @UserId AND YEAR(BudgetMonth) = YEAR(@BudgetMonth) AND MONTH(BudgetMonth) = MONTH(@BudgetMonth) AND IsDeleted = 0 
                        AND ((CategoryId IS NULL AND @CategoryId IS NULL) 
                              OR 
                            (CategoryId = @CategoryId))
                        AND (
                            @ExcludeBudgetId IS NULL
                            OR BudgetId <> @ExcludeBudgetId
                        );";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    UserId = userId,
                    CategoryId = categoryId,
                    BudgetMonth = budgetMonth,
                    ExcludeBudgetId = excludeBudgetId
                });
            return count > 0;
        }
        public async Task Update(BudgetLimit budget)
        {
            var sql = @"UPDATE BudgetLimit SET CategoryId = @CategoryId, BudgetAmount = @BudgetAmount, BudgetMonth=@BudgetMonth, UpdatedAt = GETDATE() WHERE BudgetId = @BudgetId AND IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, budget);
        }
        public async Task SoftDelete(int budgetId)
        {
            var sql = @"UPDATE BudgetLimit SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE BudgetId = @BudgetId AND IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                sql,
                new { BudgetId = budgetId });
        }
    }
}
