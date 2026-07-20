import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { ApiError } from '../../../core/models/auth.model';

@Component({
    selector: 'app-check-email',
    standalone: true,
    imports: [CommonModule, RouterLink],
    templateUrl: './check-email.component.html'
})
export class CheckEmailComponent {
    private router = inject(Router);
    private auth = inject(AuthService);

    userId = signal<number | null>(null);
    email = signal<string>('');
    editingEmail = signal(false);
    newEmail = signal('');

    loading = signal(false);
    successMessage = signal('');
    errorMessage = signal('');

    // Separate state for the "resend by email" fallback section
    resendByEmailInput = signal('');
    resendByEmailLoading = signal(false);
    resendByEmailMessage = signal('');
    constructor() {
        const state = history.state as { userId?: number; email?: string };

        if (state?.userId) {
            // Came from registration — full resend/change-email support
            this.userId.set(state.userId);
            this.email.set(state.email ?? '');
            this.newEmail.set(state.email ?? '');
        } else if (state?.email) {
            // Came from a failed login (email-not-verified) — no userId available
            this.email.set(state.email);
            this.newEmail.set(state.email);
        }
        // else: no state at all (refresh, direct link, or failed verify-email
        // redirect) — just render the page with the manual resend-by-email
        // section empty, letting the user type their email in themselves.
    }
    toggleEditEmail() {
        this.editingEmail.update(v => !v);
        this.successMessage.set('');
        this.errorMessage.set('');
    }

    resend() {
        if (this.userId() === null) return;

        this.loading.set(true);
        this.successMessage.set('');
        this.errorMessage.set('');

        const emailToUse = this.editingEmail() ? this.newEmail() : this.email();

        this.auth.resendVerification({ userId: this.userId()!, email: emailToUse }).subscribe({
            next: () => {
                this.loading.set(false);
                this.email.set(emailToUse);
                this.editingEmail.set(false);
                this.successMessage.set('Verification email sent! Please check your inbox.');
            },
            error: (err: HttpErrorResponse) => {
                this.loading.set(false);
                const apiErr = err.error as ApiError;
                this.errorMessage.set(apiErr?.message ?? 'Failed to resend verification email.');
            }
        });
    }
    resendByEmail() {
        const emailToUse = this.resendByEmailInput().trim();
        if (!emailToUse) return;

        this.resendByEmailLoading.set(true);
        this.resendByEmailMessage.set('');

        this.auth.resendVerificationByEmail({ email: emailToUse }).subscribe({
            next: () => {
                this.resendByEmailLoading.set(false);
                this.resendByEmailMessage.set(
                    'If an unverified account exists for this email, a verification email has been sent.'
                );
            },
            error: () => {
                this.resendByEmailLoading.set(false);
                this.resendByEmailMessage.set(
                    'Something went wrong. Please try again.'
                );
            }
        });
    }
}