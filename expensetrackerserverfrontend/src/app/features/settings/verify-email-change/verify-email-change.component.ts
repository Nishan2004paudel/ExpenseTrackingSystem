import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { SettingsService } from '../../../core/services/settings.service';
import { ApiError } from '../../../core/models/auth.model';

type VerifyState = 'loading' | 'success' | 'error';

@Component({
  selector: 'app-verify-email-change',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './verify-email-change.component.html'
})
export class VerifyEmailChangeComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private settings = inject(SettingsService);

  state = signal<VerifyState>('loading');
  errorMessage = signal('');

  ngOnInit() {
    const token = this.route.snapshot.queryParamMap.get('token');

    if (!token) {
      this.state.set('error');
      this.errorMessage.set('Missing verification token.');
      return;
    }

    this.settings.verifyEmailChange(token).subscribe({
      next: () => {
        this.state.set('success');
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2500);
      },
      error: (err: HttpErrorResponse) => {
        this.state.set('error');
        const apiErr = err.error as ApiError;
        this.errorMessage.set(apiErr?.message ?? 'Verification failed. The link may be invalid or expired.');
      }
    });
  }
}