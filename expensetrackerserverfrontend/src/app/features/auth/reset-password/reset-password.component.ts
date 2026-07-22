import { Component, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { ApiError } from '../../../core/models/auth.model';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './reset-password.component.html'
})
export class ResetPasswordComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  token = signal<string | null>(null);
  showPassword = signal(false);
  loading = signal(false);
  success = signal(false);
  errorMessage = signal('');

  form = this.fb.group({
    password: ['', Validators.required]
  });

  private passwordValue = signal('');

  hasMinLength = computed(() => this.passwordValue().length >= 8);
  hasUppercase = computed(() => /[A-Z]/.test(this.passwordValue()));
  hasLowercase = computed(() => /[a-z]/.test(this.passwordValue()));
  hasDigit = computed(() => /[0-9]/.test(this.passwordValue()));
  isPasswordValid = computed(() =>
    this.hasMinLength() && this.hasUppercase() && this.hasLowercase() && this.hasDigit()
  );

  constructor() {
    const t = this.route.snapshot.queryParamMap.get('token');
    if (!t) {
      this.errorMessage.set('Missing reset token. Please request a new password reset link.');
    }
    this.token.set(t);
  }

  onPasswordInput(value: string) {
    this.passwordValue.set(value);
  }

  togglePassword() {
    this.showPassword.update(v => !v);
  }

  onSubmit() {
    if (!this.token() || !this.isPasswordValid()) return;

    this.loading.set(true);
    this.errorMessage.set('');

    this.auth.resetPassword({ token: this.token()!, password: this.passwordValue() }).subscribe({
      next: () => {
        this.loading.set(false);
        this.success.set(true);
        setTimeout(() => this.router.navigate(['/login']), 2500);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const apiErr = err.error as ApiError;
        this.errorMessage.set(apiErr?.message ?? 'Failed to reset password. Please try again.');
      }
    });
  }
}