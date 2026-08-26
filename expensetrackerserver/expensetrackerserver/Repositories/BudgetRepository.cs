using Dapper;
using expensetrackerserver.Data;
using expensetrackerserver.Models;
using expensetrackerserver.DTOs;

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

        public async Task TransferCategory(int sourceCategoryId, int targetCategoryId)
        {
            var sql = @"UPDATE source SET source.CategoryId = @TargetCategoryId, source.UpdatedAt = GETDATE() FROM BudgetLimit source 
                            WHERE source.CategoryId = @SourceCategoryId AND source.IsDeleted = 0
                            AND NOT EXISTS 
                            (
                        SELECT 1 
                        FROM BudgetLimit target
                        WHERE 
                            target.CategoryId = @TargetCategoryId
                            AND target.IsDeleted = 0
                            AND YEAR(target.BudgetMonth) = YEAR(source.BudgetMonth)
                            AND MONTH(target.BudgetMonth) = MONTH(source.BudgetMonth))
                        ;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                SourceCategoryId = sourceCategoryId,
                TargetCategoryId = targetCategoryId
            });
        }

        public async Task SoftDeleteByCategory(int categoryId)
        {
            var sql = @"UPDATE BudgetLimit SET IsDeleted = 1,UpdatedAt = GETDATE() WHERE CategoryId = @CategoryId AND IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                CategoryId = categoryId
            });
        }

        public async Task<IEnumerable<BudgetConflictDto>> GetConflictingBudgets(int sourceCategoryId, int targetCategoryId)
        {
            var sql = @"SELECT
                            s.BudgetMonth,

                            s.CategoryId AS SourceCategoryId,
                            sc.CategoryName AS SourceCategoryName,

                            t.CategoryId AS TargetCategoryId,
                            tc.CategoryName AS TargetCategoryName,

                            s.BudgetAmount AS SourceBudgetAmount,
                            t.BudgetAmount AS TargetBudgetAmount

                        FROM BudgetLimit s
                        INNER JOIN BudgetLimit t
                            ON YEAR(s.BudgetMonth) = YEAR(t.BudgetMonth)
                            AND MONTH(s.BudgetMonth) = MONTH(t.BudgetMonth)

                        INNER JOIN Category sc
                            ON sc.CategoryId = s.CategoryId

                        INNER JOIN Category tc
                            ON tc.CategoryId = t.CategoryId

                        WHERE
                            s.CategoryId = @SourceCategoryId
                            AND t.CategoryId = @TargetCategoryId
                            AND s.IsDeleted = 0
                            AND t.IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<BudgetConflictDto>(sql, new
            {
                SourceCategoryId = sourceCategoryId,
                TargetCategoryId = targetCategoryId
            });
        }

        public async Task MergeConflictingBudgets(int sourceCategoryId, int targetCategoryId)
        {
            var sql = @"UPDATE target SET target.BudgetAmount =target.BudgetAmount+ source.BudgetAmount ,
                                    target.UpdatedAt = GETDATE()
                                FROM  BudgetLimit target
                                INNER JOIN BudgetLimit source 
                        ON YEAR(target.BudgetMonth)=YEAR(source.BudgetMonth)
                        AND MONTH(target.BudgetMonth)=MONTH(source.BudgetMonth)
                        WHERE 
                            target.CategoryId = @TargetCategoryId
                            AND source.CategoryId = @SourceCategoryId
                            AND target.IsDeleted = 0
                            AND source.IsDeleted = 0;
                        UPDATE BudgetLimit 
                        SET 
                            IsDeleted = 1,
                            UpdatedAt = GETDATE()
                       WHERE BudgetId IN 
                        (
                            SELECT source.BudgetId 
                            FROM BudgetLimit source
                            INNER JOIN BudgetLimit target
                                ON YEAR(target.BudgetMonth)=YEAR(source.BudgetMonth)
                                AND MONTH(target.BudgetMonth)=MONTH(source.BudgetMonth)
                        WHERE 
                            source.CategoryId = @SourceCategoryId
                            AND target.CategoryId = @TargetCategoryId
                            AND source.IsDeleted = 0
                            AND target.IsDeleted = 0);";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                SourceCategoryId = sourceCategoryId,
                TargetCategoryId = targetCategoryId
            });
        }
        public async Task DeleteConflictingSourceBudgets(int sourceCategoryId, int targetCategoryId)
        {
            var sql = @"UPDATE source SET source.IsDeleted = 1, source.UpdatedAt = GETDATE() FROM BudgetLimit source 
                        INNER JOIN BudgetLimit target 
                        ON YEAR(target.BudgetMonth)=YEAR(source.BudgetMonth)
                        AND MONTH(target.BudgetMonth)=MONTH(source.BudgetMonth)
                        WHERE  source.CategoryId = @SourceCategoryId
                        AND target.CategoryId = @TargetCategoryId
                        AND source.IsDeleted = 0
                        AND target.IsDeleted = 0;";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                SourceCategoryId = sourceCategoryId,
                TargetCategoryId = targetCategoryId
            });


        }

        public async Task<BudgetUsageDto?> GetBudgetUsage(int userId, int categoryId, DateTime expenseDate)
        {
            var sql = @"SELECT b.BudgetAmount, ISNULL(SUM(e.Amount),0) AS ExpenseAmount FROM BudgetLimit b LEFT JOIN Expense e ON e.UserId = b.UserId
                        AND e.CategoryId = b.CategoryId AND e.IsDeleted = 0 AND YEAR(e.ExpenseDate) = YEAR(@ExpenseDate) AND MONTH(e.ExpenseDate) = MONTH(@ExpenseDate)
                        WHERE b.UserId = @UserId AND b.CategoryId = @CategoryId AND b.BudgetMonth = DATEFROMPARTS(YEAR(@ExpenseDate), MONTH(@ExpenseDate),1) AND b.IsDeleted =0 GROUP BY b.BudgetAmount;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<BudgetUsageDto>(sql,
                new
                {
                    UserId = userId,
                    CategoryId = categoryId,
                    ExpenseDate = expenseDate
                });
        }
    }
}
