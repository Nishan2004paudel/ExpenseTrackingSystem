import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-terms',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <main class="flex min-h-screen items-center justify-center bg-[radial-gradient(circle_at_top_left,_rgba(103,80,164,0.16),_transparent_35%),linear-gradient(135deg,_#f8fafc_0%,_#eef2ff_100%)] px-4 py-10 sm:px-6 lg:px-8">
      <div class="w-full max-w-3xl rounded-[28px] border border-outline-variant/70 bg-surface-container-lowest/95 p-6 shadow-[0_20px_70px_rgba(15,23,42,0.08)] backdrop-blur-xl sm:p-8 lg:p-10">
        <div class="mb-6 flex items-center justify-between gap-3">
          <div>
            <p class="text-sm font-semibold uppercase tracking-[0.24em] text-primary">Legal</p>
            <h1 class="text-2xl font-semibold tracking-tight text-on-background">Terms of Service</h1>
          </div>
          <a routerLink="/register" class="text-sm font-medium text-primary transition hover:underline">Back to create account</a>
        </div>

        <div class="space-y-4 text-sm leading-7 text-on-surface-variant">
          <p>Welcome to HisabKitab. By using our service, you agree to the following simple terms.</p>
          <p>You are responsible for keeping your account credentials secure and for the activity that happens under your account.</p>
          <p>Use the app for lawful personal budgeting and expense tracking. Do not misuse the platform or attempt to interfere with its operation.</p>
          <p>We may update these terms from time to time. Continued use of the service means you accept the latest version.</p>
          <p>If you have questions, please reach out to our support team through the app.</p>
        </div>
      </div>
    </main>
  `
})
export class TermsComponent {}
