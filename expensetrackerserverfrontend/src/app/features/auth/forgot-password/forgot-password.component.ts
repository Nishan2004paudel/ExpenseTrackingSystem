import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { ApiError } from '../../../core/models/auth.model';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.component.html'
})
export class ForgotPasswordComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);

  loading = signal(false);
  submitted = signal(false);
  errorMessage = signal('');
  form = this.fb.group({
    identifier: ['', Validators.required]
  });

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');

    this.auth.forgotPassword(this.form.value as any).subscribe({
      next: () => {
        this.loading.set(false);
        this.submitted.set(true);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const apiErr = err.error as ApiError;

        if (apiErr?.message?.toLowerCase().includes('verify your email')) {
          // Legitimate, distinct case — tell them clearly, don't hide behind
          // the generic "check your email" message
          this.errorMessage.set(apiErr.message);
          return;
        }

        // Any other error (network issue, server down, etc.) — still show
        // the generic confirmation to avoid leaking account existence
        this.submitted.set(true);
      }
    });
  }
}