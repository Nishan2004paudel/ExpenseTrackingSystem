import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { ApiError } from '../../../core/models/auth.model';

declare const google: any;

@Component({
  selector: 'app-google-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './google-button.component.html'
})
export class GoogleButtonComponent implements OnInit {
  @Input() label = 'Continue with Google';

  private auth = inject(AuthService);
  private router = inject(Router);

  errorMessage = signal('');

  ngOnInit() {
    google.accounts.id.initialize({
      client_id: '465620580623-ck8l4ditrcmie6fj0teo563a4aabols3.apps.googleusercontent.com',
      callback: (response: any) => this.handleCredential(response.credential)
    });

    google.accounts.id.renderButton(
      document.getElementById('google-btn-container'),
      {
        theme: 'outline',
        size: 'large',
        width: 380,
        text: this.label === 'Continue with Google' ? 'continue_with' : 'signup_with'
      }
    );
  }

  private handleCredential(idToken: string) {
    this.errorMessage.set('');

    this.auth.googleLogin({ idToken }).subscribe({
      next: () => this.router.navigate(['/profile']),
      error: (err: HttpErrorResponse) => {
        const apiErr = err.error as ApiError;
        this.errorMessage.set(apiErr?.message ?? 'Google sign-in failed. Please try again.');
      }
    });
  }
}