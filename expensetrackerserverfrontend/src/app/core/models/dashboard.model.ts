export interface DashboardSummary {
  budgetAmount?: number;
  expenseAmount: number;
  remainingAmount?: number;
  percentageUsed?: number;
}

export interface MonthlyExpenseSummary {
  year: number;
  month: number;
  monthName: string;
  budgetAmount?: number;
  expenseAmount: number;
  remainingAmount?: number;
  percentageUsed?: number;
}

export interface MonthlyCategorySummary {
  categoryId: number;
  categoryName: string;
  budgetAmount?: number;
  expenseAmount: number;
  remainingAmount?: number;
  percentageUsed?: number;
}

export interface CategoryExpenseSummary {
  categoryId: number;
  categoryName: string;
  budgetAmount?: number;
  expenseAmount: number;
  remainingAmount?: number;
  percentageUsed?: number;
}

export interface CategoryMonthlySummary {
  year: number;
  month: number;
  monthName: string;
  budgetAmount?: number;
  expenseAmount: number;
  remainingAmount?: number;
  percentageUsed?: number;
}