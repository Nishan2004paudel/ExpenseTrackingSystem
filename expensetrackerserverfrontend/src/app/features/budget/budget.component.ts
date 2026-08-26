import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { BudgetService } from '../../core/services/budget.service';
import { CategoryService } from '../../core/services/category.service';
import { Budget } from '../../core/models/budget.model';
import { Category } from '../../core/models/category.model';
import { ApiError } from '../../core/models/auth.model';

function currentMonthIso(): string {
  const d = new Date();
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-01`;
}

@Component({
  selector: 'app-budget',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './budget.component.html'
})
export class BudgetComponent implements OnInit {
  private budgetService = inject(BudgetService);
  private categoryService = inject(CategoryService);
  private route = inject(ActivatedRoute);

  budgets = signal<Budget[]>([]);
  categories = signal<Category[]>([]);
  loading = signal(true);
  loadError = signal('');

  minMonth = currentMonthIso();

  totalBudget = computed(() =>
    this.budgets().reduce((sum, b) => sum + b.budgetAmount, 0)
  );

  // Create/Edit form
  showForm = signal(false);
  editingBudgetId = signal<number | null>(null);
  formCategoryId = signal<number | null>(null); // null = overall budget
  formAmount = signal<number | null>(null);
  formMonth = signal<string>(currentMonthIso());
  formLoading = signal(false);
  formError = signal('');

  // Delete state
  deletingId = signal<number | null>(null);
  deleteError = signal('');

  ngOnInit() {
    this.fetchCategories();
    this.fetchBudgets();

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

  fetchBudgets() {
    this.loading.set(true);
    this.loadError.set('');

    this.budgetService.getAll().subscribe({
      next: (budgets) => {
        this.loading.set(false);
        this.budgets.set(budgets);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const apiErr = err.error as ApiError;
        this.loadError.set(apiErr?.message ?? 'Failed to load budgets.');
      }
    });
  }

  openCreateForm() {
    this.editingBudgetId.set(null);
    this.formCategoryId.set(null);
    this.formAmount.set(null);
    this.formMonth.set(currentMonthIso());
    this.formError.set('');
    this.showForm.set(true);
  }

  openEditForm(budget: Budget) {
    this.editingBudgetId.set(budget.budgetId);
    this.formCategoryId.set(budget.categoryId ?? null);
    this.formAmount.set(budget.budgetAmount);
    this.formMonth.set(budget.budgetMonth.slice(0, 7) + '-01');
    this.formError.set('');
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
  }

  submitForm() {
    const amount = this.formAmount();
    const budgetMonth = this.formMonth();
    if (amount === null || amount <= 0 || !budgetMonth) return;

    this.formLoading.set(true);
    this.formError.set('');

    const payload = {
      categoryId: this.formCategoryId() ?? undefined,
      budgetAmount: amount,
      budgetMonth
    };

    const editingId = this.editingBudgetId();
    const request$ = editingId
      ? this.budgetService.update(editingId, payload)
      : this.budgetService.create(payload);

    request$.subscribe({
      next: (budget) => {
        this.formLoading.set(false);
        this.showForm.set(false);

        if (editingId) {
          this.budgets.update(list =>
            list.map(b => b.budgetId === editingId ? budget : b)
          );
        } else {
          this.budgets.update(list => [budget, ...list]);
        }
      },
      error: (err: HttpErrorResponse) => {
        this.formLoading.set(false);
        const apiErr = err.error as ApiError;
        this.formError.set(apiErr?.message ?? 'Failed to save budget.');
      }
    });
  }

  deleteBudget(budgetId: number) {
    this.deletingId.set(budgetId);
    this.deleteError.set('');

    this.budgetService.delete(budgetId).subscribe({
      next: () => {
        this.deletingId.set(null);
        this.budgets.update(list => list.filter(b => b.budgetId !== budgetId));
      },
      error: (err: HttpErrorResponse) => {
        this.deletingId.set(null);
        const apiErr = err.error as ApiError;
        this.deleteError.set(apiErr?.message ?? 'Failed to delete budget.');
      }
    });
  }
}