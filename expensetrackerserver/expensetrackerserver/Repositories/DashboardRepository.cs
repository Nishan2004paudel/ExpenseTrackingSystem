using Dapper;
using expensetrackerserver.Data;
using expensetrackerserver.Models;

namespace expensetrackerserver.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DapperContext _context;
        public DashboardRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummary> GetSummary(
            int userId,
            int year,
            int? month,
            int? categoryId)
        {
            var sql = @"SELECT ISNULL(
                            ( 
                            SELECT SUM(BudgetAmount)
                            FROM BudgetLimit WHERE UserId = @UserId AND
                            YEAR(BudgetMonth) = @Year
                            AND IsDeleted = 0

                            AND (
                                @Month IS NULL
                                OR MONTH(BudgetMonth) = @Month
                            )

                            AND(
                                (CategoryId IS NULL AND @CategoryId IS NULL)
                                OR
                                (CategoryId = @CategoryId)
                            )
                        ),0) AS BudgetAmount,

                        ISNULL
                        (
                        (
                            SELECT SUM(Amount)
                            FROM Expense 
                            WHERE UserId = @UserId 
                            AND IsDeleted = 0
                            AND YEAR(ExpenseDate) = @Year

                            AND
                            (
                                @Month IS NULL
                                OR MONTH(ExpenseDate) = @Month
                            )

                            AND 
                            (
                                @CategoryId IS NULL
                                OR CategoryId = @CategoryId
                            )
                        ),0) AS ExpenseAmount;
                        ";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<DashboardSummary>(
                sql,
                new
                {
                    UserId = userId,
                    Year = year,
                    Month = month,
                    CategoryId = categoryId
                });
        }

        public async Task<IEnumerable<MonthlyExpenseSummary>> GetExpenseByMonth(int userId, int year)
        {
            var sql = @"WITH Months AS 
                        (
                            SELECT 1 AS MonthNumber
                            UNION ALL SELECT 2
                            UNION ALL SELECT 3
                            UNION ALL SELECT 4
                            UNION ALL SELECT 5
                            UNION ALL SELECT 6
                            UNION ALL SELECT 7
                            UNION ALL SELECT 8
                            UNION ALL SELECT 9
                            UNION ALL SELECT 10
                            UNION ALL SELECT 11
                            UNION ALL SELECT 12
                        )
                        SELECT 
                            @Year AS Year,
                            m.MonthNumber AS Month,
                            b.BudgetAmount,
                        
                            ISNULL(e.ExpenseAmount,0) AS ExpenseAmount
                        FROM Months m
                        CROSS JOIN [User] u
                            
                        LEFT JOIN 
                        (
                            SELECT 
                                MONTH(BudgetMonth) AS BudgetMonthNumber,
                                SUM(BudgetAmount) AS BudgetAmount
                            FROM BudgetLimit
                            WHERE UserId = @UserId
                                AND CategoryId IS NULL
                                AND YEAR(BudgetMonth) = @Year
                                AND IsDeleted = 0
                            GROUP BY MONTH(BudgetMonth)
                          )b
                                ON b.BudgetMonthNumber = m.MonthNumber

                        LEFT JOIN(
                                   SELECT 
                                        MONTH(ExpenseDate) AS ExpenseMonthNumber,
                                        SUM(Amount) As ExpenseAmount
                                    FROM Expense
                                    WHERE UserId = @UserId
                                        AND YEAR(ExpenseDate) = @Year
                                        AND IsDeleted = 0
                                    GROUP BY MONTH(ExpenseDate)
                        )e
                        ON e.ExpenseMonthNumber = m.MonthNumber
                        WHERE u.UserId = @UserId
                        AND
                        (
                             @Year > YEAR(u.CreatedAt)

                                 OR

                              (
                                     @Year = YEAR(u.CreatedAt)

                                      AND

                                       m.MonthNumber>=MONTH(u.CreatedAt)
                               )
                            )

                            AND
                            (
                                 @Year < YEAR(GETDATE())

                                 OR

                                 (
                                     @Year=YEAR(GETDATE())

                                      AND

                                         m.MonthNumber<=MONTH(GETDATE())
                                    )
                                )
                                ORDER BY m.MonthNumber;";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<MonthlyExpenseSummary>(
                sql,
                new
                {
                    UserId = userId,
                    Year = year
                });
        }

        public async Task<IEnumerable<MonthlyCategorySummary>> GetMonthBreakdown(int userId, int year, int month, bool includeEmpty = false)
        {
            var sql = @"SELECT c.CategoryId, c.CategoryName, b.BudgetAmount, ISNULL(e.ExpenseAmount,0) AS ExpenseAmount FROM Category c
                          LEFT JOIN
                            (SELECT
                                    CategoryId,
                                    SUM(BudgetAmount) AS BudgetAmount
                               FROM BudgetLimit
                                WHERE UserId = @UserId
                                    AND YEAR(BudgetMonth) = @Year
                                    AND MONTH(BudgetMonth) = @Month
                                    AND IsDeleted = 0
                                    AND CategoryId IS NOT NULL
                                GROUP BY CategoryId) b
                                ON c.CategoryId = b.CategoryId
                           LEFT JOIN
                                (
                                    SELECT CategoryId, SUM(Amount) AS ExpenseAmount
                                    FROM Expense
                                    WHERE UserId = @UserId
                                        AND YEAR(ExpenseDate) = @Year
                                        AND MONTH(ExpenseDate) = @Month
                                        AND IsDeleted = 0
                                    GROUP BY CategoryId) e
                                   ON c.CategoryId = e.CategoryId
                            WHERE 
                                c.UserId = @UserId
                            AND c.IsDeleted = 0
                            AND
                                (
                                    @IncludeEmpty = 1
                                    OR 
                                    b.BudgetAmount IS NOT NULL
                                    OR
                                    e.ExpenseAmount IS NOT NULL
                                ) 
                           ORDER BY ISNULL(e.ExpenseAmount,0) DESC,c.CategoryName;";

            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<MonthlyCategorySummary>(
                sql,
                new
                {
                    UserId = userId,
                    Year = year,
                    Month = month,
                    IncludeEmpty = includeEmpty
                });
        }


        public async Task<IEnumerable<CategoryExpenseSummary>> GetExpenseByCategory(int userId, int year, bool includeEmpty = false)
        {
            var sql = @"SELECT c.CategoryId,
                                c.CategoryName,
                                b.BudgetAmount,
                                ISNULL(e.ExpenseAmount,0) AS ExpenseAmount
                        FROM Category c

                        LEFT JOIN
                        (
                            SELECT 
                                CategoryId,
                                SUM(BudgetAmount) AS BudgetAmount
                            FROM BudgetLimit
                            WHERE UserId = @UserId
                                AND YEAR(BudgetMonth) = @Year
                                AND IsDeleted = 0
                                AND CategoryId IS NOT NULL
                            GROUP BY CategoryId 
                        )b 
                            ON c.CategoryId = b.CategoryId

                        LEFT JOIN
                        (
                            SELECT
                                CategoryId, 
                                SUM(Amount) AS ExpenseAmount
                            FROM Expense
                            WHERE UserId = @UserId
                                AND YEAR(ExpenseDate) = @Year
                                AND IsDeleted = 0
                            GROUP BY CategoryId
                        )e
                            ON c.CategoryId = e.CategoryId

                        WHERE
                            c.UserId = @UserId
                            AND c.IsDeleted = 0
                            AND 
                            (
                                @IncludeEmpty = 1
                                OR 
                                b.BudgetAmount IS NOT NULL
                                OR
                                e.ExpenseAmount IS NOT NULL
                            )
                        ORDER BY
                            ISNULL(e.ExpenseAmount,0) DESC,
                            c.CategoryName;";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<CategoryExpenseSummary>(
                sql,
                new
                {
                    UserId = userId,
                    Year = year,
                    IncludeEmpty = includeEmpty
                });
        }

        public async Task<IEnumerable<CategoryMonthlySummary>> GetCategoryBreakdown(int userId, int year, int categoryId)
        {
            var sql = @"
                    WITH Months AS 
                    (
                        SELECT MONTH (BudgetMonth) AS MonthNumber
                        FROM BudgetLimit
                        WHERE UserId = @UserId
                            AND CategoryId = @CategoryId
                            AND YEAR(BudgetMonth) = @Year
                            AND IsDeleted = 0
                        UNION
                        SELECT MONTH (ExpenseDate)
                        FROM Expense
                        WHERE UserId = @UserId
                            AND CategoryId = @CategoryId
                            AND YEAR(ExpenseDate) = @Year
                            AND IsDeleted = 0
                    )
                    
                    SELECT 
                        @Year AS Year,  
                        m.MonthNumber AS Month,
                        b.BudgetAmount,
                        ISNULL(e.ExpenseAmount,0) AS ExpenseAmount

                    FROM Months m

                    LEFT JOIN
                    (
                        SELECT
                            MONTH(BudgetMonth) AS BudgetMonthNumber,
                            SUM(BudgetAmount) AS BudgetAmount
                        FROM BudgetLimit
                        WHERE UserId = @UserId
                            AND CategoryId = @CategoryId 
                            AND YEAR(BudgetMonth) = @Year
                            AND IsDeleted = 0
                        GROUP BY MONTH(BudgetMonth)
                    )b
                        ON b.BudgetMonthNumber = m.MonthNumber

                    LEFT JOIN
                    (
                        SELECT 
                            MONTH(ExpenseDate) AS ExpenseMonthNumber,
                            SUM(Amount) AS ExpenseAmount
                        FROM Expense
                        WHERE UserId = @UserId
                            AND CategoryId = @CategoryId
                            AND YEAR(ExpenseDate) = @Year
                            AND IsDeleted = 0
                        GROUP BY MONTH(ExpenseDate)
                    ) e
                        ON e.ExpenseMonthNumber = m.MonthNumber
                    ORDER BY m.MonthNumber;";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<CategoryMonthlySummary>(
                sql,
                new
                {
                    UserId = userId,
                    Year = year,
                    CategoryId = categoryId
                });
        }
    }
}
