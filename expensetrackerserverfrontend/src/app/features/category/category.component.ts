import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { CategoryService } from '../../core/services/category.service';
import {
  Category,
  CategoryDeleteAction,
  BudgetConflictAction,
  BudgetConflict
} from '../../core/models/category.model';
import { ApiError } from '../../core/models/auth.model';

type DeleteStep = 'choose-action' | 'pick-target' | 'name-new' | 'resolve-conflicts';

@Component({
  selector: 'app-category',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './category.component.html'
})
export class CategoryComponent implements OnInit {
  private categoryService = inject(CategoryService);

  CategoryDeleteAction = CategoryDeleteAction;
  BudgetConflictAction = BudgetConflictAction;

  categories = signal<Category[]>([]);
  loading = signal(true);
  loadError = signal('');

  // Create form
  newCategoryName = signal('');
  createLoading = signal(false);
  createError = signal('');

  // Edit state (per-row)
  editingId = signal<number | null>(null);
  editCategoryName = signal('');
  editLoading = signal(false);
  editError = signal('');

  // --- Delete flow state ---
  deleteModalCategory = signal<Category | null>(null);
  deleteStep = signal<DeleteStep>('choose-action');
  selectedAction = signal<CategoryDeleteAction | null>(null);
  targetCategoryId = signal<number | null>(null);
  newCategoryNameForTransfer = signal('');
  conflicts = signal<BudgetConflict[]>([]);
  selectedConflictAction = signal<BudgetConflictAction | null>(null);
  deleteLoading = signal(false);
  deleteError = signal('');

  availableTargets = computed(() => {
    const current = this.deleteModalCategory();
    return this.categories().filter(c => c.categoryId !== current?.categoryId);
  });

  ngOnInit() {
    this.fetchCategories();
  }

  fetchCategories() {
    this.loading.set(true);
    this.loadError.set('');

    this.categoryService.getAll().subscribe({
      next: (categories) => {
        this.loading.set(false);
        this.categories.set(categories);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const apiErr = err.error as ApiError;
        this.loadError.set(apiErr?.message ?? 'Failed to load categories.');
      }
    });
  }

  createCategory() {
    const categoryName = this.newCategoryName().trim();
    if (!categoryName) return;

    this.createLoading.set(true);
    this.createError.set('');

    this.categoryService.create({ categoryName }).subscribe({
      next: (category) => {
        this.createLoading.set(false);
        this.categories.update(list => [category, ...list]);
        this.newCategoryName.set('');
      },
      error: (err: HttpErrorResponse) => {
        this.createLoading.set(false);
        const apiErr = err.error as ApiError;
        this.createError.set(apiErr?.message ?? 'Failed to create category.');
      }
    });
  }

  startEdit(category: Category) {
    this.editingId.set(category.categoryId);
    this.editCategoryName.set(category.categoryName);
    this.editError.set('');
  }

  cancelEdit() {
    this.editingId.set(null);
    this.editError.set('');
  }

  submitEdit(categoryId: number) {
    const categoryName = this.editCategoryName().trim();
    if (!categoryName) return;

    this.editLoading.set(true);
    this.editError.set('');

    this.categoryService.update(categoryId, { categoryName }).subscribe({
      next: (updated) => {
        this.editLoading.set(false);
        this.categories.update(list =>
          list.map(c => c.categoryId === categoryId ? updated : c)
        );
        this.editingId.set(null);
      },
      error: (err: HttpErrorResponse) => {
        this.editLoading.set(false);
        const apiErr = err.error as ApiError;
        this.editError.set(apiErr?.message ?? 'Failed to update category.');
      }
    });
  }

  // --- Delete flow ---

  openDeleteModal(category: Category) {
    this.deleteModalCategory.set(category);
    this.deleteStep.set('choose-action');
    this.selectedAction.set(null);
    this.targetCategoryId.set(null);
    this.newCategoryNameForTransfer.set('');
    this.conflicts.set([]);
    this.selectedConflictAction.set(null);
    this.deleteError.set('');
  }

  closeDeleteModal() {
    this.deleteModalCategory.set(null);
  }

  chooseAction(action: CategoryDeleteAction) {
    this.selectedAction.set(action);
    this.deleteError.set('');

    if (action === CategoryDeleteAction.DeleteAll) {
      this.confirmDelete();
    } else if (action === CategoryDeleteAction.TransferToExisting) {
      this.deleteStep.set('pick-target');
    } else if (action === CategoryDeleteAction.TransferToNew) {
      this.deleteStep.set('name-new');
    }
  }

  confirmDelete() {
    const category = this.deleteModalCategory();
    const action = this.selectedAction();
    if (!category || action === null) return;

    this.deleteLoading.set(true);
    this.deleteError.set('');

    this.categoryService.delete(category.categoryId, {
      action,
      targetCategoryId: action === CategoryDeleteAction.TransferToExisting
        ? this.targetCategoryId() ?? undefined
        : undefined,
      newCategoryName: action === CategoryDeleteAction.TransferToNew
        ? this.newCategoryNameForTransfer().trim()
        : undefined,
      conflictAction: this.selectedConflictAction() ?? undefined
    }).subscribe({
      next: (result) => {
        this.deleteLoading.set(false);

        if (result && result.conflicts.length > 0) {
          // Backend paused — conflicts need resolving before we can proceed
          this.conflicts.set(result.conflicts);
          this.deleteStep.set('resolve-conflicts');
          return;
        }

        // Success — refetch since a new category may have been created,
        // or budgets/expenses may have moved (any local patch would be incomplete)
        this.fetchCategories();
        this.closeDeleteModal();
      },
      error: (err: HttpErrorResponse) => {
        this.deleteLoading.set(false);
        const apiErr = err.error as ApiError;
        this.deleteError.set(apiErr?.message ?? 'Failed to delete category.');
      }
    });
  }

  resolveConflict(action: BudgetConflictAction) {
    this.selectedConflictAction.set(action);
    this.confirmDelete();
  }
}