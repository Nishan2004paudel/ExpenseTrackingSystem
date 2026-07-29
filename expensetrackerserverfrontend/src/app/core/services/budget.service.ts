import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Budget, CreateBudgetRequest, UpdateBudgetRequest } from '../models/budget.model';

@Injectable({ providedIn: 'root' })
export class BudgetService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/budget`;

  getAll() {
    return this.http.get<Budget[]>(this.baseUrl);
  }

  getById(budgetId: number) {
    return this.http.get<Budget>(`${this.baseUrl}/${budgetId}`);
  }

  create(payload: CreateBudgetRequest) {
    return this.http.post<Budget>(this.baseUrl, payload);
  }

  update(budgetId: number, payload: UpdateBudgetRequest) {
    return this.http.put<Budget>(`${this.baseUrl}/${budgetId}`, payload);
  }

  delete(budgetId: number) {
    return this.http.delete<void>(`${this.baseUrl}/${budgetId}`);
  }
}