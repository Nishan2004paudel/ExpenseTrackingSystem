export interface Category {
  categoryId: number;
  categoryName: string;
}

export interface CreateCategoryRequest {
  categoryName: string;
}

export interface UpdateCategoryRequest {
  categoryName: string;
}

export enum CategoryDeleteAction {
  DeleteAll = 1,
  TransferToExisting = 2,
  TransferToNew = 3
}

export enum BudgetConflictAction {
  Merge = 1,
  DeleteSource = 2
}

export interface DeleteCategoryRequest {
  action: CategoryDeleteAction;
  targetCategoryId?: number;
  newCategoryName?: string;
  conflictAction?: BudgetConflictAction;
}

export interface BudgetConflict {
  budgetMonth: string; // ISO date string from backend DateTime
  sourceCategoryId: number;
  sourceCategoryName: string;
  targetCategoryId: number;
  targetCategoryName: string;
  sourceBudgetAmount: number;
  targetBudgetAmount: number;
}

export interface CategoryDeleteConflict {
  conflicts: BudgetConflict[];
}