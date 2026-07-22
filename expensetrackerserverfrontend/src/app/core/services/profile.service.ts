import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';
import {
  UserDetail,
  SetupPasswordRequest,
  SetupUsernameRequest,
  UpdatePreferredCalendarRequest,
  UpdateFullNameRequest,
  UpdateProfessionRequest
} from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  getMe() {
    return this.http
      .get<UserDetail>(`${environment.apiUrl}/profile/me`)
      .pipe(tap(user => this.auth.currentUser.set(user)));
  }

  setupPassword(payload: SetupPasswordRequest) {
    return this.http.post(`${environment.apiUrl}/profile/setup-password`, payload);
  }

  setupUsername(payload: SetupUsernameRequest) {
    return this.http.post(`${environment.apiUrl}/profile/setup-username`, payload);
  }

  updatePreferredCalendar(payload: UpdatePreferredCalendarRequest) {
    return this.http.put(`${environment.apiUrl}/profile/preferred-calendar`, payload);
  }
  updateFullName(payload: UpdateFullNameRequest) {
    return this.http.put(`${environment.apiUrl}/profile/full-name`, payload);
  }

  updateProfession(payload: UpdateProfessionRequest) {
    return this.http.put(`${environment.apiUrl}/profile/profession`, payload);
  }
}