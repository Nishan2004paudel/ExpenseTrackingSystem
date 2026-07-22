import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';
import { ApiError } from '../../core/models/auth.model';
import { ProfileService } from '../../core/services/profile.service';
@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html'
})
export class ProfileComponent {
  auth = inject(AuthService);
  private profile = inject(ProfileService);
  private router = inject(Router);
  // Username setup
  showUsernameForm = signal(false);
  usernameInput = signal('');
  usernameLoading = signal(false);
  usernameError = signal('');

  // Password setup
  showPasswordForm = signal(false);
  passwordInput = signal('');
  passwordLoading = signal(false);
  passwordError = signal('');
  showPasswordText = signal(false);

  hasMinLength = computed(() => this.passwordInput().length >= 8);
  hasUppercase = computed(() => /[A-Z]/.test(this.passwordInput()));
  hasLowercase = computed(() => /[a-z]/.test(this.passwordInput()));
  hasDigit = computed(() => /[0-9]/.test(this.passwordInput()));
  isPasswordValid = computed(() =>
    this.hasMinLength() && this.hasUppercase() && this.hasLowercase() && this.hasDigit()
  );

  // Preferred calendar
  calendarLoading = signal(false);
  calendarMessage = signal('');

  // Full name editing
  showFullNameForm = signal(false);
  fullNameInput = signal('');
  fullNameLoading = signal(false);
  fullNameError = signal('');

  // Profession editing
  showProfessionForm = signal(false);
  professionInput = signal('');
  professionLoading = signal(false);
  professionError = signal('');

  togglePasswordText() {
    this.showPasswordText.update(v => !v);
  }

  submitUsername() {
    const username = this.usernameInput().trim();
    if (!username) return;

    this.usernameLoading.set(true);
    this.usernameError.set('');

    this.profile.setupUsername({ username }).subscribe({
      next: () => {
        this.usernameLoading.set(false);
        this.showUsernameForm.set(false);
        const current = this.auth.currentUser();
        if (current) {
          this.auth.currentUser.set({ ...current, username });
        }
      },
      error: (err: HttpErrorResponse) => {
        this.usernameLoading.set(false);
        const apiErr = err.error as ApiError;
        this.usernameError.set(apiErr?.message ?? 'Failed to set username.');
      }
    });
  }

  submitPassword() {
    if (!this.isPasswordValid()) return;

    this.passwordLoading.set(true);
    this.passwordError.set('');

    this.profile.setupPassword({ password: this.passwordInput() }).subscribe({
      next: () => {
        this.passwordLoading.set(false);
        this.showPasswordForm.set(false);
        this.passwordInput.set('');
        const current = this.auth.currentUser();
        if (current) {
          this.auth.currentUser.set({ ...current, hasPassword: true });
        }
      },
      error: (err: HttpErrorResponse) => {
        this.passwordLoading.set(false);
        const apiErr = err.error as ApiError;
        this.passwordError.set(apiErr?.message ?? 'Failed to set password.');
      }
    });
  }

  changeCalendar(preferredCalendar: string) {
    this.calendarLoading.set(true);
    this.calendarMessage.set('');

    this.profile.updatePreferredCalendar({ preferredCalendar }).subscribe({
      next: () => {
        this.calendarLoading.set(false);
        this.calendarMessage.set('Updated!');
        const current = this.auth.currentUser();
        if (current) {
          this.auth.currentUser.set({ ...current, preferredCalendar });
        }
        setTimeout(() => this.calendarMessage.set(''), 2000);
      },
      error: () => {
        this.calendarLoading.set(false);
        this.calendarMessage.set('Failed to update.');
      }
    });
  }
  startEditFullName() {
    this.fullNameInput.set(this.auth.currentUser()?.fullName ?? '');
    this.fullNameError.set('');
    this.showFullNameForm.set(true);
  }

  submitFullName() {
    const fullName = this.fullNameInput().trim();
    if (!fullName) return;

    this.fullNameLoading.set(true);
    this.fullNameError.set('');

    this.profile.updateFullName({ fullName }).subscribe({
      next: () => {
        this.fullNameLoading.set(false);
        this.showFullNameForm.set(false);
        const current = this.auth.currentUser();
        if (current) {
          this.auth.currentUser.set({ ...current, fullName });
        }
      },
      error: (err: HttpErrorResponse) => {
        this.fullNameLoading.set(false);
        const apiErr = err.error as ApiError;
        this.fullNameError.set(apiErr?.message ?? 'Failed to update full name.');
      }
    });
  }

  startEditProfession() {
    this.professionInput.set(this.auth.currentUser()?.profession ?? '');
    this.professionError.set('');
    this.showProfessionForm.set(true);
  }

  submitProfession() {
    const profession = this.professionInput().trim();

    this.professionLoading.set(true);
    this.professionError.set('');

    this.profile.updateProfession({ profession: profession || undefined }).subscribe({
      next: () => {
        this.professionLoading.set(false);
        this.showProfessionForm.set(false);
        const current = this.auth.currentUser();
        if (current) {
          this.auth.currentUser.set({ ...current, profession: profession || undefined });
        }
      },
      error: (err: HttpErrorResponse) => {
        this.professionLoading.set(false);
        const apiErr = err.error as ApiError;
        this.professionError.set(apiErr?.message ?? 'Failed to update profession.');
      }
    });
  }
  logout() {
    this.auth.logout().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login'])
    });
  }
  logoutEverywhere() {
    this.auth.logoutEverywhere().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login'])
    });
  }
}