import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AdminUser } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/admin`;

  getAllUsers() {
    return this.http.get<AdminUser[]>(`${this.baseUrl}/Users`);
  }

  getUserById(userId: number) {
    return this.http.get<AdminUser>(`${this.baseUrl}/users/${userId}`);
  }

  deactivateUser(userId: number) {
    return this.http.post<{ message: string }>(`${this.baseUrl}/users/${userId}/deactivate`, {});
  }

  activateUser(userId: number) {
    return this.http.post<{ message: string }>(`${this.baseUrl}/users/${userId}/activate`, {});
  }

  deleteUser(userId: number) {
    return this.http.delete<void>(`${this.baseUrl}/users/${userId}`);
  }
}