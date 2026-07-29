import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Category,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  DeleteCategoryRequest,
  CategoryDeleteConflict
} from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/category`;

  getAll() {
    return this.http.get<Category[]>(this.baseUrl);
  }

  create(payload: CreateCategoryRequest) {
    return this.http.post<Category>(this.baseUrl, payload);
  }

  update(categoryId: number, payload: UpdateCategoryRequest) {
    return this.http.put<Category>(`${this.baseUrl}/${categoryId}`, payload);
  }

  /**
   * Returns:
   * - null when the backend responds 204 (deletion/transfer completed successfully)
   * - CategoryDeleteConflict when the backend responds 200 with conflicting budgets
   *   (nothing was deleted/transferred yet — caller must resolve and call again)
   */
  delete(categoryId: number, payload: DeleteCategoryRequest): Observable<CategoryDeleteConflict | null> {
    return this.http.delete<CategoryDeleteConflict | null>(
      `${this.baseUrl}/${categoryId}`,
      { body: payload, observe: 'body' }
    );
  }
}