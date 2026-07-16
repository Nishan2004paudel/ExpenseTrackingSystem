import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  RegisterRequest,
  LoginRequest,
  LoginResponse,
  UserDetail,
  ResendVerificationRequest,
  ResendByEmailRequest
} from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  // Access token lives ONLY in memory — cleared on refresh/tab close
  private accessToken: string | null = null;

  isAuthenticated = signal<boolean>(false);
  currentUser = signal<UserDetail | null>(null);

  constructor(private http: HttpClient) {}

  register(payload: RegisterRequest) {
    return this.http.post<UserDetail>(
      `${environment.apiUrl}/auth/register`,
      payload
    );
  }

  login(payload: LoginRequest) {
    return this.http
      .post<LoginResponse>(
        `${environment.apiUrl}/auth/login`,
        payload,
        { withCredentials: true }
      )
      .pipe(tap(res => this.setSession(res)));
  }

  private setSession(res: LoginResponse) {
    this.accessToken = res.accessToken;
    this.currentUser.set(res.user);
    this.isAuthenticated.set(true);
  }

  getToken(): string | null {
    return this.accessToken;
  }

  logout() {
    return this.http
      .post(`${environment.apiUrl}/auth/logout`, {}, { withCredentials: true })
      .pipe(tap(() => this.clearSession()));
  }

  clearSession() {
    this.accessToken = null;
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
  }
  resendVerification(payload: ResendVerificationRequest) {
    return this.http.post(
      `${environment.apiUrl}/auth/resend-verification`,
      payload
    );
  }
  resendVerificationByEmail(payload: ResendByEmailRequest) {
    return this.http.post(
      `${environment.apiUrl}/auth/resend-verification-by-email`,
      payload
    );
  }
}