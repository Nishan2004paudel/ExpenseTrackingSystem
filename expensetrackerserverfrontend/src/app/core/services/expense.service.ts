import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  Expense,
  CreateExpenseRequest,
  UpdateExpenseRequest,
  ExpenseFilters
} from '../models/expense.model';

@Injectable({ providedIn: 'root' })
export class ExpenseService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/expense`;

  getAll(filters?: ExpenseFilters) {
    let params = new HttpParams();
    if (filters?.year != null) params = params.set('year', filters.year);
    if (filters?.month != null) params = params.set('month', filters.month);
    if (filters?.categoryId != null) params = params.set('categoryId', filters.categoryId);

    return this.http.get<Expense[]>(this.baseUrl, { params });
  }

  getById(expenseId: number) {
    return this.http.get<Expense>(`${this.baseUrl}/${expenseId}`);
  }

  create(payload: CreateExpenseRequest) {
    return this.http.post<Expense>(this.baseUrl, payload);
  }

  update(expenseId: number, payload: UpdateExpenseRequest) {
    return this.http.put<Expense>(`${this.baseUrl}/${expenseId}`, payload);
  }

  delete(expenseId: number) {
    return this.http.delete<void>(`${this.baseUrl}/${expenseId}`);
  }
}