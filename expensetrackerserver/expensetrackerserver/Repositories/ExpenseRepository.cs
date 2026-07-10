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

        public async Task<IEnumerable<ExpenseWithCategory>> GetFilteredExpenses(int userId, int? year, int? month, int? categoryId)
        {
            var sql = @"
                        SELECT e.ExpenseId,e.UserId,e.CategoryId,c.CategoryName,e.Amount,e.ExpenseDate,e.Description FROM Expense e
                        INNER JOIN Category c
                                ON  e.CategoryId = c.CategoryId
                        WHERE e.UserId = @UserId
                        AND e.IsDeleted = 0
                        AND
                        (
                            @Year IS NULL
                            OR YEAR(e.ExpenseDate)=@Year
                        )
                        AND 
                        (
                            @Month IS NULL
                            OR MONTH(e.ExpenseDate) = @Month
                        )
                        AND 
                        (
                            @CategoryId IS NULL
                            OR e.CategoryId = @CategoryId
                        )

                        ORDER BY 
                            e.ExpenseDate DESC,
                            e.ExpenseId DESC;";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<ExpenseWithCategory>(
                sql,
                new
                {
                    UserId = userId,
                    Year = year,
                    Month = month,
                    CategoryId = categoryId
                });
        }
    }
}
