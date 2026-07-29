export interface Expense {
  expenseId: number;
  categoryId: number;
  categoryName: string;
  amount: number;
  expenseDate: string; // ISO date string
  description?: string;
}

export interface CreateExpenseRequest {
  categoryId: number;
  amount: number;
  expenseDate: string; // ISO date string, e.g. '2026-01-15'
  description?: string;
}

export interface UpdateExpenseRequest {
  categoryId: number;
  amount: number;
  expenseDate: string;
  description?: string;
}

export interface ExpenseFilters {
  year?: number;
  month?: number;
  categoryId?: number;
}