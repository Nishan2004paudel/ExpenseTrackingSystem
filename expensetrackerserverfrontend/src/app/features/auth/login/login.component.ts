import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { ApiError } from '../../../core/models/auth.model';
import { GoogleButtonComponent } from '../google-button/google-button.component';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterLink, GoogleButtonComponent],
    templateUrl: './login.component.html'
})
export class LoginComponent {
    private fb = inject(FormBuilder);
    private auth = inject(AuthService);
    private router = inject(Router);
    private route = inject(ActivatedRoute);

    showPassword = signal(false);
    loading = signal(false);
    errorMessage = signal('');
    justRegistered = signal(false);

    form = this.fb.group({
        identifier: ['', Validators.required],
        password: ['', Validators.required]
    });

    constructor() {
        this.justRegistered.set(this.route.snapshot.queryParamMap.get('registered') === 'true');
    }

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

        this.auth.login(this.form.value as any).subscribe({
            next: () => {
                this.loading.set(false);
                this.router.navigate(['/profile']);
            },
            error: (err: HttpErrorResponse) => {
                this.loading.set(false);
                const apiErr = err.error as ApiError;
                const message = apiErr?.message ?? 'Invalid credentials. Please try again.';

                if (message.toLowerCase().includes('verify your email')) {
                    const identifier = this.form.value.identifier ?? '';
                    const looksLikeEmail = identifier.includes('@');

                    this.router.navigate(['/check-email'], {
                        state: { email: looksLikeEmail ? identifier : '' }
                    });
                } else {
                    this.errorMessage.set(message);
                }
            }
        });
    }
}