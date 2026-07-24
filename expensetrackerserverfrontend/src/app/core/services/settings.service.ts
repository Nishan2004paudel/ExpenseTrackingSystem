import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ChangeEmailRequest, ChangePasswordRequest, ChangeUsernameRequest } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private http = inject(HttpClient);

  changeEmail(payload: ChangeEmailRequest) {
    return this.http.put<{ message: string }>(`${environment.apiUrl}/settings/email`, payload);
  }

  verifyEmailChange(token: string) {
    return this.http.get<{ message: string }>(`${environment.apiUrl}/settings/verify-email-change`, {
      params: { token }
    });
  }
  changePassword(payload: ChangePasswordRequest) {
    return this.http.put<{ message: string }>(`${environment.apiUrl}/settings/change-password`, payload);
  }
  changeUsername(payload: ChangeUsernameRequest) {
    return this.http.put<{ message: string }>(`${environment.apiUrl}/settings/username`, payload);
  }
}