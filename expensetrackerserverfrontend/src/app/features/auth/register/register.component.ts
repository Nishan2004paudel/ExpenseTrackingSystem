import { Component, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { ApiError } from '../../../core/models/auth.model';
import { GoogleButtonComponent } from '../google-button/google-button.component';


function strongPasswordValidator(control: AbstractControl): ValidationErrors | null {
  const value: string = control.value ?? '';
  const hasUpper = /[A-Z]/.test(value);
  const hasLower = /[a-z]/.test(value);
  const hasDigit = /[0-9]/.test(value);
  return hasUpper && hasLower && hasDigit ? null : { weakPassword: true };
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, GoogleButtonComponent],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = signal(false);
  errorMessage = signal('');
  fieldErrors = signal<Record<string, string[]>>({});
  showPassword = signal(false);

  professions = ['Student', 'Engineer', 'Teacher', 'Business Owner', 'Freelancer', 'Other'];

  form = this.fb.group({
    fullName: ['', [Validators.required, Validators.maxLength(100)]],
    username: ['', [Validators.required, Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
    password: ['', [Validators.required, Validators.minLength(8), strongPasswordValidator]],
    preferredCalendar: ['English', Validators.required],
    profession: [''],
    agreeToTerms: [false, Validators.requiredTrue]
  });
  private passwordValue = toSignal(
    this.form.controls.password.valueChanges,
    { initialValue: '' }
  );

  hasMinLength = computed(() => (this.passwordValue() ?? '').length >= 8);
  hasUppercase = computed(() => /[A-Z]/.test(this.passwordValue() ?? ''));
  hasLowercase = computed(() => /[a-z]/.test(this.passwordValue() ?? ''));
  hasDigit = computed(() => /[0-9]/.test(this.passwordValue() ?? ''));
  togglePassword() {
    this.showPassword.update(v => !v);
  }
  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');
    this.fieldErrors.set({});

    const { agreeToTerms, ...payload } = this.form.value;

    this.auth.register(payload as any).subscribe({
      next: (user) => {
        this.loading.set(false);
        this.router.navigate(['/check-email'], {
          state: { userId: user.userId, email: user.email }
        });
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const apiErr = err.error as ApiError;
        if (apiErr?.errors) {
          this.fieldErrors.set(apiErr.errors);
        }
        this.errorMessage.set(apiErr?.message ?? 'Registration failed. Please try again.');
      }
    });
  }
}