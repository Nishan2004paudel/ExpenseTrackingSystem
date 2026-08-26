import { Injectable, signal } from '@angular/core';

export interface RegisterFormState {
  fullName: string;
  username: string;
  email: string;
  password: string;
  preferredCalendar: string;
  profession: string;
  agreeToTerms: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class RegisterFormStateService {
  private readonly state = signal<RegisterFormState | null>(null);

  getState(): RegisterFormState | null {
    return this.state();
  }

  saveState(state: Partial<RegisterFormState> | null): void {
    if (!state) {
      this.state.set(null);
      return;
    }

    this.state.set({
      fullName: state.fullName ?? '',
      username: state.username ?? '',
      email: state.email ?? '',
      password: state.password ?? '',
      preferredCalendar: state.preferredCalendar ?? 'English',
      profession: state.profession ?? '',
      agreeToTerms: Boolean(state.agreeToTerms)
    });
  }

  clearState(): void {
    this.state.set(null);
  }
}
