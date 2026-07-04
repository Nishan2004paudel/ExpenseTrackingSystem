using Dapper;
using expensetrackerserver.Data;
using expensetrackerserver.Models;


namespace expensetrackerserver.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly DapperContext _context;
        public ExpenseRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> Create(Expense expense)
        {
            var sql = @"INSERT INTO Expense (UserId, CategoryId, Amount, ExpenseDate, Description) VALUES (@UserId, @CategoryId, @Amount, @ExpenseDate, @Description);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, expense);
        }
        public async Task<IEnumerable<ExpenseWithCategory>> GetAllByUserId(int userId)
        {
            var sql = @"SELECT e.ExpenseId, e.UserId,e.CategoryId, c.CategoryName,e.Amount,e.ExpenseDate,e.Description FROM Expense e INNER JOIN Category c ON e.CategoryId = c.CategoryId WHERE e.UserId = @UserId AND e.IsDeleted = 0 ORDER BY e.ExpenseDate DESC, e.CreatedAt DESC;";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<ExpenseWithCategory>(
                sql,
                new { UserId = userId });
        }
        public async Task<ExpenseWithCategory?> GetById(int expenseId)
        {
            var sql = @"SELECT e.ExpenseId,e.UserId, e.CategoryId, c.CategoryName,e.Amount,e.ExpenseDate, e.Description FROM Expense e INNER JOIN Category c ON e.CategoryId = c.CategoryId WHERE e.ExpenseId = @ExpenseId AND e.IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<ExpenseWithCategory>(
                sql,
                new { ExpenseId = expenseId });
        }

        public async Task<bool> HasActiveExpenses(int categoryId)
        {
            var sql = @"SELECT COUNT(1) FROM Expense WHERE CategoryId = @CategoryId AND IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                sql, new { CategoryId = categoryId });
            return count > 0;
        }
        public async Task Update(Expense expense)
        {
            var sql = @"UPDATE Expense SET CategoryId = @CategoryId, Amount = @Amount, ExpenseDate = @ExpenseDate, Description= @Description,UpdatedAt = GETDATE() WHERE ExpenseId = @ExpenseId AND IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, expense);
        }
        public async Task SoftDelete(int expenseId)
        {
            var sql = @"UPDATE Expense SET IsDeleted=1, UpdatedAt = GETDATE() WHERE ExpenseId = @ExpenseId AND IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                sql,
                new { ExpenseId = expenseId });
        }
    }
}
