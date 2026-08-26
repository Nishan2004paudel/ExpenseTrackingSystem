import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  DashboardSummary,
  MonthlyExpenseSummary,
  MonthlyCategorySummary,
  CategoryExpenseSummary,
  CategoryMonthlySummary
} from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/dashboard`;

  getSummary(year: number, month?: number, categoryId?: number) {
    let params = new HttpParams().set('year', year);
    if (month != null) params = params.set('month', month);
    if (categoryId != null) params = params.set('categoryId', categoryId);
    return this.http.get<DashboardSummary>(`${this.baseUrl}/summary`, { params });
  }

  getExpenseByMonth(year: number) {
    const params = new HttpParams().set('year', year);
    return this.http.get<MonthlyExpenseSummary[]>(`${this.baseUrl}/months`, { params });
  }

  getMonthBreakdown(year: number, month: number, includeEmpty = false) {
    const params = new HttpParams()
      .set('year', year).set('month', month).set('includeEmpty', includeEmpty);
    return this.http.get<MonthlyCategorySummary[]>(`${this.baseUrl}/month/breakdown`, { params });
  }

  getExpenseByCategory(year: number, includeEmpty = false) {
    const params = new HttpParams().set('year', year).set('includeEmpty', includeEmpty);
    return this.http.get<CategoryExpenseSummary[]>(`${this.baseUrl}/categories`, { params });
  }

  getCategoryBreakdown(year: number, categoryId: number) {
    const params = new HttpParams().set('year', year).set('categoryId', categoryId);
    return this.http.get<CategoryMonthlySummary[]>(`${this.baseUrl}/categories/breakdown`, { params });
  }
}