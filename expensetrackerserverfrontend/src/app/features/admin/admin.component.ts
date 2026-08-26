import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AdminService } from '../../core/services/admin.service';
import { AuthService } from '../../core/services/auth.service';
import { AdminUser } from '../../core/models/admin.model';
import { ApiError } from '../../core/models/auth.model';

@Component({
  selector: 'app-admin',
  standalone: true,
   imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html'
})
export class AdminComponent implements OnInit {
  private adminService = inject(AdminService);
  auth = inject(AuthService);

  users = signal<AdminUser[]>([]);
  loading = signal(true);
  loadError = signal('');

  actionLoadingId = signal<number | null>(null);
  actionError = signal('');

  showDeleteConfirmId = signal<number | null>(null);
  deleteConfirmText = signal('');

  ngOnInit() {
    this.fetchUsers();
  }

  fetchUsers() {
    this.loading.set(true);
    this.loadError.set('');

    this.adminService.getAllUsers().subscribe({
      next: (users) => {
        this.loading.set(false);
        this.users.set(users);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const apiErr = err.error as ApiError;
        this.loadError.set(apiErr?.message ?? 'Failed to load users.');
      }
    });
  }

  isSelf(userId: number): boolean {
    return this.auth.currentUser()?.userId === userId;
  }

  deactivate(userId: number) {
    this.actionLoadingId.set(userId);
    this.actionError.set('');

    this.adminService.deactivateUser(userId).subscribe({
      next: () => {
        this.actionLoadingId.set(null);
        this.users.update(list =>
          list.map(u => u.userId === userId ? { ...u, isActive: false, deactivationReason: 'Admin' } : u)
        );
      },
      error: (err: HttpErrorResponse) => {
        this.actionLoadingId.set(null);
        const apiErr = err.error as ApiError;
        this.actionError.set(apiErr?.message ?? 'Failed to deactivate user.');
      }
    });
  }

  activate(userId: number) {
    this.actionLoadingId.set(userId);
    this.actionError.set('');

    this.adminService.activateUser(userId).subscribe({
      next: () => {
        this.actionLoadingId.set(null);
        this.users.update(list =>
          list.map(u => u.userId === userId ? { ...u, isActive: true, deactivationReason: undefined } : u)
        );
      },
      error: (err: HttpErrorResponse) => {
        this.actionLoadingId.set(null);
        const apiErr = err.error as ApiError;
        this.actionError.set(apiErr?.message ?? 'Failed to activate user.');
      }
    });
  }

  startDelete(userId: number) {
    this.showDeleteConfirmId.set(userId);
    this.deleteConfirmText.set('');
    this.actionError.set('');
  }

  cancelDelete() {
    this.showDeleteConfirmId.set(null);
    this.deleteConfirmText.set('');
  }

  confirmDelete(userId: number) {
    if (this.deleteConfirmText().trim().toUpperCase() !== 'DELETE') return;

    this.actionLoadingId.set(userId);
    this.actionError.set('');

    this.adminService.deleteUser(userId).subscribe({
      next: () => {
        this.actionLoadingId.set(null);
        this.showDeleteConfirmId.set(null);
        this.users.update(list => list.filter(u => u.userId !== userId));
      },
      error: (err: HttpErrorResponse) => {
        this.actionLoadingId.set(null);
        const apiErr = err.error as ApiError;
        this.actionError.set(apiErr?.message ?? 'Failed to delete user.');
      }
    });
  }
}