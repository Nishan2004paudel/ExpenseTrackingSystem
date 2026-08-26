import { Component, signal, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { ApiError } from '../../../core/models/auth.model';
import { GoogleButtonComponent } from '../google-button/google-button.component';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink, GoogleButtonComponent],
    templateUrl: './login.component.html'
})
export class LoginComponent implements OnDestroy {
    private fb = inject(FormBuilder);
    private auth = inject(AuthService);
    private router = inject(Router);
    private route = inject(ActivatedRoute);
    private notifications = inject(NotificationService);

    showPassword = signal(false);
    loading = signal(false);
    errorMessage = signal('');
    justRegistered = signal(false);
    rateLimitSecondsLeft = signal(0);
    showReactivatePrompt = signal(false);
    reactivatePasswordInput = signal('');
    reactivateLoading = signal(false);
    reactivateError = signal('');
    reactivateSuccess = signal(false);
    private rateLimitTimer?: ReturnType<typeof setInterval>;
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
    private startRateLimitCountdown(seconds: number) {
        this.rateLimitSecondsLeft.set(seconds);
        this.errorMessage.set('');

        this.rateLimitTimer = setInterval(() => {
            const remaining = this.rateLimitSecondsLeft() - 1;
            if (remaining <= 0) {
                this.rateLimitSecondsLeft.set(0);
                clearInterval(this.rateLimitTimer);
            } else {
                this.rateLimitSecondsLeft.set(remaining);
            }
        }, 1000);
    }

    ngOnDestroy() {
        if (this.rateLimitTimer) {
            clearInterval(this.rateLimitTimer);
        }
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
                this.notifications.startConnection();
                this.router.navigate(['/dashboard']);
            },
            error: (err: HttpErrorResponse) => {
                this.loading.set(false);
                this.showReactivatePrompt.set(false);

                if (err.status === 429) {
                    this.startRateLimitCountdown(60);
                    return;
                }

                const apiErr = err.error as ApiError;
                const message = apiErr?.message ?? 'Invalid credentials. Please try again.';
                const lowerMessage = message.toLowerCase();

                if (lowerMessage.includes('verify your email')) {
                    const identifier = this.form.value.identifier ?? '';
                    const looksLikeEmail = identifier.includes('@');

                    this.router.navigate(['/check-email'], {
                        state: { email: looksLikeEmail ? identifier : '' }
                    });
                    return;
                }

                // Deactivated-by-self accounts can self-reactivate right here.
                // Admin-deactivated accounts get the same error text but no
                // reactivate option, since only an admin can undo that.
                if (lowerMessage.includes('reactivate your account to continue')) {
                    this.showReactivatePrompt.set(true);
                }

                this.errorMessage.set(message);
            }
        });
    }
    submitReactivate() {
        const identifier = this.form.value.identifier ?? '';
        const password = this.reactivatePasswordInput();
        if (!identifier || !password) return;

        this.reactivateLoading.set(true);
        this.reactivateError.set('');

        this.auth.reactivateAccount({ identifier, password }).subscribe({
            next: () => {
                this.reactivateLoading.set(false);
                this.reactivateSuccess.set(true);
                this.showReactivatePrompt.set(false);
                this.errorMessage.set('');
            },
            error: (err: HttpErrorResponse) => {
                this.reactivateLoading.set(false);
                const apiErr = err.error as ApiError;
                this.reactivateError.set(apiErr?.message ?? 'Failed to reactivate account.');
            }
        });
    }
}