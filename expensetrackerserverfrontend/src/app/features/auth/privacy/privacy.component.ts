import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-privacy',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <main class="flex min-h-screen items-center justify-center bg-[radial-gradient(circle_at_top_left,_rgba(103,80,164,0.16),_transparent_35%),linear-gradient(135deg,_#f8fafc_0%,_#eef2ff_100%)] px-4 py-10 sm:px-6 lg:px-8">
      <div class="w-full max-w-3xl rounded-[28px] border border-outline-variant/70 bg-surface-container-lowest/95 p-6 shadow-[0_20px_70px_rgba(15,23,42,0.08)] backdrop-blur-xl sm:p-8 lg:p-10">
        <div class="mb-6 flex items-center justify-between gap-3">
          <div>
            <p class="text-sm font-semibold uppercase tracking-[0.24em] text-primary">Legal</p>
            <h1 class="text-2xl font-semibold tracking-tight text-on-background">Privacy Policy</h1>
          </div>
          <a routerLink="/register" class="text-sm font-medium text-primary transition hover:underline">Back to create account</a>
        </div>

        <div class="space-y-4 text-sm leading-7 text-on-surface-variant">
          <p>We collect only the information needed to create your account, authenticate you, and provide the budgeting experience.</p>
          <p>This may include your name, email address, password, and expense-related data you choose to enter.</p>
          <p>Your information is used to operate the app, keep your account secure, and improve the service over time.</p>
          <p>We do not sell your personal data. You may contact us if you want to review or update your information.</p>
        </div>
      </div>
    </main>
  `
})
export class PrivacyComponent {}
