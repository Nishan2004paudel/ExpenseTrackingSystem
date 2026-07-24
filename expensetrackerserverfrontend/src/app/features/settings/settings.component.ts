import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { SettingsService } from '../../core/services/settings.service';
import { ApiError } from '../../core/models/auth.model';
@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.component.html'
})
export class SettingsComponent {
  auth = inject(AuthService);
  private settings = inject(SettingsService);
  private router = inject(Router);

  showEmailForm = signal(false);
  currentPasswordInput = signal('');
  newEmailInput = signal('');
  emailLoading = signal(false);
  emailError = signal('');
  emailSuccess = signal('');

  startChangeEmail() {
    this.currentPasswordInput.set('');
    this.newEmailInput.set('');
    this.emailError.set('');
    this.emailSuccess.set('');
    this.showEmailForm.set(true);
  }

  showPasswordForm = signal(false);
  currentPasswordForPwInput = signal('');
  newPasswordInput = signal('');
  confirmPasswordInput = signal('');
  showNewPasswordText = signal(false);
  passwordChangeLoading = signal(false);
  passwordChangeError = signal('');
  passwordChangeSuccess = signal(false);

  hasMinLength = computed(() => this.newPasswordInput().length >= 8);
  hasUppercase = computed(() => /[A-Z]/.test(this.newPasswordInput()));
  hasLowercase = computed(() => /[a-z]/.test(this.newPasswordInput()));
  hasDigit = computed(() => /[0-9]/.test(this.newPasswordInput()));
  isNewPasswordValid = computed(() =>
    this.hasMinLength() && this.hasUppercase() && this.hasLowercase() && this.hasDigit()
  );
  passwordsMatch = computed(() =>
    this.newPasswordInput().length > 0 && this.newPasswordInput() === this.confirmPasswordInput()
  );

  startChangePassword() {
    this.currentPasswordForPwInput.set('');
    this.newPasswordInput.set('');
    this.confirmPasswordInput.set('');
    this.passwordChangeError.set('');
    this.showPasswordForm.set(true);
  }

  showUsernameChangeForm = signal(false);
  currentPasswordForUsernameInput = signal('');
  newUsernameInput = signal('');
  usernameChangeLoading = signal(false);
  usernameChangeError = signal('');
  usernameChangeSuccess = signal(false);

  startChangeUsername() {
    this.currentPasswordForUsernameInput.set('');
    this.newUsernameInput.set('');
    this.usernameChangeError.set('');
    this.usernameChangeSuccess.set(false);
    this.showUsernameChangeForm.set(true);
  }

  submitChangeUsername() {
    const currentPassword = this.currentPasswordForUsernameInput();
    const newUsername = this.newUsernameInput().trim();
    if (!currentPassword || !newUsername) return;

    this.usernameChangeLoading.set(true);
    this.usernameChangeError.set('');

    this.settings.changeUsername({ currentPassword, newUsername }).subscribe({
      next: () => {
        this.usernameChangeLoading.set(false);
        this.showUsernameChangeForm.set(false);
        this.usernameChangeSuccess.set(true);
        const current = this.auth.currentUser();
        if (current) {
          this.auth.currentUser.set({ ...current, username: newUsername });
        }
        setTimeout(() => this.usernameChangeSuccess.set(false), 2500);
      },
      error: (err: HttpErrorResponse) => {
        this.usernameChangeLoading.set(false);
        const apiErr = err.error as ApiError;
        this.usernameChangeError.set(apiErr?.message ?? 'Failed to change username.');
      }
    });
  }
  logoutEverywhere() {
    this.auth.logoutEverywhere().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login'])
    });
  }

  toggleNewPasswordText() {
    this.showNewPasswordText.update(v => !v);
  }

  submitChangePassword() {
    if (!this.currentPasswordForPwInput() || !this.isNewPasswordValid() || !this.passwordsMatch()) return;

    this.passwordChangeLoading.set(true);
    this.passwordChangeError.set('');

    this.settings.changePassword({
      currentPassword: this.currentPasswordForPwInput(),
      newPassword: this.newPasswordInput(),
      confirmPassword: this.confirmPasswordInput()
    }).subscribe({
      next: () => {
        this.passwordChangeLoading.set(false);
        this.showPasswordForm.set(false);
        this.passwordChangeSuccess.set(true);
        setTimeout(() => {
          this.auth.clearSession();
          this.router.navigate(['/login'], { queryParams: { passwordChanged: 'true' } });
        }, 2000);
      },
      error: (err: HttpErrorResponse) => {
        this.passwordChangeLoading.set(false);
        const apiErr = err.error as ApiError;
        this.passwordChangeError.set(apiErr?.message ?? 'Failed to change password.');
      }
    });
  }

  private isValidEmail(value: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
  }

  submitChangeEmail() {
    const currentPassword = this.currentPasswordInput();
    const newEmail = this.newEmailInput().trim();
    if (!currentPassword || !newEmail) return;

    if (!this.isValidEmail(newEmail)) {
      this.emailError.set('Please enter a valid email address.');
      return;
    }

    this.emailLoading.set(true);
    this.emailError.set('');
    this.emailSuccess.set('');

    this.settings.changeEmail({ currentPassword, newEmail }).subscribe({
      next: () => {
        this.emailLoading.set(false);
        this.emailSuccess.set('Verification email sent to your new address. Please check your inbox to confirm the change.');
        this.showEmailForm.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.emailLoading.set(false);
        const apiErr = err.error as ApiError;
        this.emailError.set(apiErr?.message ?? 'Failed to change email.');
      }
    });
  }
}