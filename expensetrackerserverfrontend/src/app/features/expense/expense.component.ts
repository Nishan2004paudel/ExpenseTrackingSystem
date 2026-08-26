import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { ExpenseService } from '../../core/services/expense.service';
import { CategoryService } from '../../core/services/category.service';
import { Expense } from '../../core/models/expense.model';
import { Category } from '../../core/models/category.model';
import { ApiError } from '../../core/models/auth.model';

function todayIso(): string {
  const d = new Date();
  return d.toISOString().slice(0, 10);
}

@Component({
  selector: 'app-expense',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './expense.component.html'
})
export class ExpenseComponent implements OnInit {
  private expenseService = inject(ExpenseService);
  private categoryService = inject(CategoryService);
  private route = inject(ActivatedRoute);

  expenses = signal<Expense[]>([]);
  categories = signal<Category[]>([]);
  loading = signal(true);
  loadError = signal('');

  // Filters
  filterYear = signal<number | null>(new Date().getFullYear());
  filterMonth = signal<number | null>(new Date().getMonth() + 1);
  filterCategoryId = signal<number | null>(null);

  months = [
    { value: 1, label: 'January' }, { value: 2, label: 'February' }, { value: 3, label: 'March' },
    { value: 4, label: 'April' }, { value: 5, label: 'May' }, { value: 6, label: 'June' },
    { value: 7, label: 'July' }, { value: 8, label: 'August' }, { value: 9, label: 'September' },
    { value: 10, label: 'October' }, { value: 11, label: 'November' }, { value: 12, label: 'December' }
  ];
  years = computed(() => {
    const current = new Date().getFullYear();
    return Array.from({ length: 6 }, (_, i) => current - i);
  });

  totalAmount = computed(() =>
    this.expenses().reduce((sum, e) => sum + e.amount, 0)
  );

  // Create/Edit form (shared modal state)
  showForm = signal(false);
  editingExpenseId = signal<number | null>(null);
  formCategoryId = signal<number | null>(null);
  formAmount = signal<number | null>(null);
  formDate = signal<string>(todayIso());
  formDescription = signal('');
  formLoading = signal(false);
  formError = signal('');

  // Delete state
  deletingId = signal<number | null>(null);
  deleteError = signal('');

  ngOnInit() {
    this.fetchCategories();
    this.fetchExpenses();

    this.route.queryParamMap.subscribe(params => {
      const shouldOpenCreate = params.get('create') === '1';
      if (shouldOpenCreate) {
        this.openCreateForm();
      }
    });
  }

  fetchCategories() {
    this.categoryService.getAll().subscribe({
      next: (categories) => this.categories.set(categories)
    });
  }

  fetchExpenses() {
    this.loading.set(true);
    this.loadError.set('');

    this.expenseService.getAll({
      year: this.filterYear() ?? undefined,
      month: this.filterMonth() ?? undefined,
      categoryId: this.filterCategoryId() ?? undefined
    }).subscribe({
      next: (expenses) => {
        this.loading.set(false);
        this.expenses.set(expenses);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const apiErr = err.error as ApiError;
        this.loadError.set(apiErr?.message ?? 'Failed to load expenses.');
      }
    });
  }

  applyFilters() {
    this.fetchExpenses();
  }

  clearFilters() {
    this.filterYear.set(null);
    this.filterMonth.set(null);
    this.filterCategoryId.set(null);
    this.fetchExpenses();
  }

  openCreateForm() {
    this.editingExpenseId.set(null);
    this.formCategoryId.set(this.categories()[0]?.categoryId ?? null);
    this.formAmount.set(null);
    this.formDate.set(todayIso());
    this.formDescription.set('');
    this.formError.set('');
    this.showForm.set(true);
  }

  openEditForm(expense: Expense) {
    this.editingExpenseId.set(expense.expenseId);
    this.formCategoryId.set(expense.categoryId);
    this.formAmount.set(expense.amount);
    this.formDate.set(expense.expenseDate.slice(0, 10));
    this.formDescription.set(expense.description ?? '');
    this.formError.set('');
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
  }

  submitForm() {
    const categoryId = this.formCategoryId();
    const amount = this.formAmount();
    const expenseDate = this.formDate();
    if (!categoryId || amount === null || amount <= 0 || !expenseDate) return;

    this.formLoading.set(true);
    this.formError.set('');

    const payload = {
      categoryId,
      amount,
      expenseDate,
      description: this.formDescription().trim() || undefined
    };

    const editingId = this.editingExpenseId();
    const request$ = editingId
      ? this.expenseService.update(editingId, payload)
      : this.expenseService.create(payload);

    request$.subscribe({
      next: (expense) => {
        this.formLoading.set(false);
        this.showForm.set(false);

        if (editingId) {
          this.expenses.update(list =>
            list.map(e => e.expenseId === editingId ? expense : e)
          );
        } else {
          this.expenses.update(list => [expense, ...list]);
        }
      },
      error: (err: HttpErrorResponse) => {
        this.formLoading.set(false);
        const apiErr = err.error as ApiError;
        this.formError.set(apiErr?.message ?? 'Failed to save expense.');
      }
    });
  }

  deleteExpense(expenseId: number) {
    this.deletingId.set(expenseId);
    this.deleteError.set('');

    this.expenseService.delete(expenseId).subscribe({
      next: () => {
        this.deletingId.set(null);
        this.expenses.update(list => list.filter(e => e.expenseId !== expenseId));
      },
      error: (err: HttpErrorResponse) => {
        this.deletingId.set(null);
        const apiErr = err.error as ApiError;
        this.deleteError.set(apiErr?.message ?? 'Failed to delete expense.');
      }
    });
  }
}