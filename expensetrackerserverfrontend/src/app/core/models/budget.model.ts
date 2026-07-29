export interface Budget {
  budgetId: number;
  categoryId?: number;
  categoryName?: string; // undefined/null when it's an overall budget
  budgetAmount: number;
  budgetMonth: string; // ISO date string
}

export interface CreateBudgetRequest {
  categoryId?: number;
  budgetAmount: number;
  budgetMonth: string; // send as first-of-month, e.g. '2026-03-01'
}

export interface UpdateBudgetRequest {
  categoryId?: number;
  budgetAmount: number;
  budgetMonth: string;
}