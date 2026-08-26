import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';
import { ExpenseService } from '../../core/services/expense.service';
import {
  DashboardSummary,
  MonthlyExpenseSummary,
  MonthlyCategorySummary,
  CategoryExpenseSummary,
  CategoryMonthlySummary
} from '../../core/models/dashboard.model';
import { Expense } from '../../core/models/expense.model';

type ViewMode = 'by-month' | 'by-category';
type DrillLevel = 'top' | 'sub' | 'expenses';
type MonthStatus = 'past' | 'current' | 'future';

const MONTH_NAMES = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'
];

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private expenseService = inject(ExpenseService);
  private router = inject(Router);

  private readonly today = new Date();

  years = computed(() => {
    const current = new Date().getFullYear();
    return Array.from({ length: 6 }, (_, i) => current - i);
  });
  selectedYear = signal(new Date().getFullYear());

  // Top summary card
  summary = signal<DashboardSummary | null>(null);
  summaryLoading = signal(true);

  selectedSummary = signal<DashboardSummary | null>(null);
  selectedSummaryLoading = signal(false);

  // Which tab
  viewMode = signal<ViewMode>('by-month');
  drillLevel = signal<DrillLevel>('top');

  // By-month path
  yearExpenses = signal<Expense[]>([]);
  months = signal<MonthlyExpenseSummary[]>([]);
  monthsLoading = signal(true);
  selectedMonth = signal<MonthlyExpenseSummary | null>(null);
  monthCategories = signal<MonthlyCategorySummary[]>([]);
  monthCategoriesLoading = signal(false);

  // By-category path
  categories = signal<CategoryExpenseSummary[]>([]);
  categoriesLoading = signal(true);
  selectedCategory = signal<CategoryExpenseSummary | null>(null);
  categoryMonths = signal<CategoryMonthlySummary[]>([]);
  categoryMonthsLoading = signal(false);

  // Final expense list
  selectedCategoryId = signal<number | null>(null);
  selectedMonthNumber = signal<number | null>(null);
  expenses = signal<Expense[]>([]);
  expensesLoading = signal(false);

  ngOnInit() {
    this.loadAll();
  }

  loadAll() {
    this.drillLevel.set('top');
    this.clearSelection();
    this.fetchSummary();
    this.fetchMonths();
    this.fetchCategories();
    this.fetchYearExpenses();
  }

  onYearChange() {
    this.loadAll();
  }

  switchView(mode: ViewMode) {
    this.viewMode.set(mode);
    this.drillLevel.set('top');
    this.clearSelection();
  }

  fetchSummary() {
    this.summaryLoading.set(true);
    this.dashboardService.getSummary(this.selectedYear()).subscribe({
      next: (s) => { this.summaryLoading.set(false); this.summary.set(s); },
      error: () => this.summaryLoading.set(false)
    });
  }

  fetchMonths() {
    this.monthsLoading.set(true);
    this.dashboardService.getExpenseByMonth(this.selectedYear()).subscribe({
      next: (m) => {
        this.monthsLoading.set(false);
        this.months.set(this.mergeMonths(m, this.yearExpenses()));
      },
      error: () => this.monthsLoading.set(false)
    });
  }

  fetchYearExpenses() {
    this.expenseService.getAll({ year: this.selectedYear() }).subscribe({
      next: (expenses) => {
        this.yearExpenses.set(expenses);
        this.months.set(this.mergeMonths(this.months(), expenses));
      },
      error: () => this.yearExpenses.set([])
    });
  }

  fetchCategories() {
    this.categoriesLoading.set(true);
    this.dashboardService.getExpenseByCategory(this.selectedYear()).subscribe({
      next: (c) => { this.categoriesLoading.set(false); this.categories.set(c); },
      error: () => this.categoriesLoading.set(false)
    });
  }

  // --- By Month path ---
  selectMonth(month: MonthlyExpenseSummary) {
    this.clearSelection();
    this.selectedMonth.set(month);
    this.selectedMonthNumber.set(month.month);
    this.selectedCategory.set(null);
    this.selectedCategoryId.set(null);
    this.drillLevel.set('sub');
    this.monthCategoriesLoading.set(true);
    this.loadSelectionSummary(month.month);

    this.dashboardService.getMonthBreakdown(this.selectedYear(), month.month).subscribe({
      next: (c) => { this.monthCategoriesLoading.set(false); this.monthCategories.set(c); },
      error: () => this.monthCategoriesLoading.set(false)
    });
  }

  selectCategoryWithinMonth(category: MonthlyCategorySummary) {
    this.selectedCategory.set({
      categoryId: category.categoryId,
      categoryName: category.categoryName,
      budgetAmount: category.budgetAmount,
      expenseAmount: category.expenseAmount,
      remainingAmount: category.remainingAmount,
      percentageUsed: category.percentageUsed
    });
    this.selectedCategoryId.set(category.categoryId);
    this.loadSelectionSummary(this.selectedMonthNumber() ?? undefined, category.categoryId);
    this.fetchExpenses();
  }

  // --- By Category path ---
  selectCategory(category: CategoryExpenseSummary) {
    this.clearSelection();
    this.selectedCategory.set(category);
    this.selectedCategoryId.set(category.categoryId);
    this.selectedMonth.set(null);
    this.selectedMonthNumber.set(null);
    this.drillLevel.set('sub');
    this.categoryMonthsLoading.set(true);
    this.loadSelectionSummary(undefined, category.categoryId);

    this.dashboardService.getCategoryBreakdown(this.selectedYear(), category.categoryId).subscribe({
      next: (m) => { this.categoryMonthsLoading.set(false); this.categoryMonths.set(m); },
      error: () => this.categoryMonthsLoading.set(false)
    });
  }

  selectMonthWithinCategory(month: CategoryMonthlySummary) {
    this.selectedMonthNumber.set(month.month);
    this.loadSelectionSummary(month.month, this.selectedCategoryId() ?? undefined);
    this.fetchExpenses();
  }

  fetchExpenses() {
    this.drillLevel.set('expenses');
    this.expensesLoading.set(true);

    this.expenseService.getAll({
      year: this.selectedYear(),
      month: this.selectedMonthNumber() ?? undefined,
      categoryId: this.selectedCategoryId() ?? undefined
    }).subscribe({
      next: (e) => { this.expensesLoading.set(false); this.expenses.set(e); },
      error: () => this.expensesLoading.set(false)
    });
  }

  backToSub() {
    this.drillLevel.set('sub');
    this.expenses.set([]);
  }

  backToTop() {
    this.drillLevel.set('top');
    this.clearSelection();
    this.selectedMonth.set(null);
    this.selectedCategory.set(null);
    this.selectedCategoryId.set(null);
    this.selectedMonthNumber.set(null);
  }

  goToAddExpense() {
    this.router.navigate(['/expenses'], { queryParams: { create: 1 } });
  }

  goToSetBudget() {
    this.router.navigate(['/budgets'], { queryParams: { create: 1 } });
  }

  monthName(m: number) {
    return MONTH_NAMES[m - 1];
  }

  monthStatus(year: number, month: number): MonthStatus {
    if (year < this.today.getFullYear()) return 'past';
    if (year > this.today.getFullYear()) return 'future';
    if (month < this.today.getMonth() + 1) return 'past';
    if (month === this.today.getMonth() + 1) return 'current';
    return 'future';
  }

  statusLabel(year: number, month: number) {
    const status = this.monthStatus(year, month);
    return status === 'past' ? 'Past' : status === 'current' ? 'Current' : 'Future';
  }

  hasBudget(amount?: number | null) {
    return amount != null && amount > 0;
  }

  budgetLabel(amount?: number | null) {
    return this.hasBudget(amount) ? amount : null;
  }

  private clearSelection() {
    this.selectedSummary.set(null);
    this.selectedSummaryLoading.set(false);
    this.monthCategories.set([]);
    this.categoryMonths.set([]);
    this.expenses.set([]);
  }

  private loadSelectionSummary(month?: number, categoryId?: number) {
    this.selectedSummaryLoading.set(true);
    this.dashboardService.getSummary(this.selectedYear(), month, categoryId).subscribe({
      next: (summary) => {
        this.selectedSummary.set(summary);
        this.selectedSummaryLoading.set(false);
      },
      error: () => {
        this.selectedSummaryLoading.set(false);
        this.selectedSummary.set(null);
      }
    });
  }

  private mergeMonths(monthSummaries: MonthlyExpenseSummary[], expenses: Expense[]) {
    const expenseTotals = new Map<number, number>();
    for (const expense of expenses) {
      const expenseMonth = new Date(expense.expenseDate).getMonth() + 1;
      expenseTotals.set(expenseMonth, (expenseTotals.get(expenseMonth) ?? 0) + expense.amount);
    }

    const summaryByMonth = new Map<number, MonthlyExpenseSummary>();
    for (const monthSummary of monthSummaries) {
      summaryByMonth.set(monthSummary.month, monthSummary);
    }

    const mergedMonths = Array.from(new Set([...summaryByMonth.keys(), ...expenseTotals.keys()])).sort((a, b) => a - b);

    return mergedMonths.map((month) => {
      const existing = summaryByMonth.get(month);
      const expenseAmount = existing?.expenseAmount ?? expenseTotals.get(month) ?? 0;
      const budgetAmount = existing?.budgetAmount;
      const hasBudget = this.hasBudget(budgetAmount);

      return {
        year: this.selectedYear(),
        month,
        monthName: existing?.monthName ?? MONTH_NAMES[month - 1],
        expenseAmount,
        budgetAmount: hasBudget ? budgetAmount : undefined,
        remainingAmount: hasBudget && budgetAmount != null ? budgetAmount - expenseAmount : undefined,
        percentageUsed: hasBudget && budgetAmount != null ? (expenseAmount / budgetAmount) * 100 : undefined
      } satisfies MonthlyExpenseSummary;
    });
  }
}