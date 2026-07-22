export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  fullName: string;
  profession?: string;
  preferredCalendar: 'English' | 'Nepali';
}

export interface UserDetail {
  userId: number;
  username?: string;
  email: string;
  fullName: string;
  profession?: string;
  preferredCalendar: string;
  role: string;
  authProvider: string;
  hasPassword: boolean;
}

export interface LoginRequest {
  identifier: string;
  password: string;
}

export interface LoginResponse {
  user: UserDetail;
  message: string;
  accessToken: string;
  refreshToken: string;
}

export interface ApiError {
  statusCode: number;
  message: string;
  errors?: Record<string, string[]>;
}
export interface ResendVerificationRequest {
  userId: number;
  email: string;
}
export interface ResendByEmailRequest {
  email: string;
}
export interface GoogleLoginRequest {
  idToken: string;
}

export interface SetupPasswordRequest {
  password: string;
}

export interface SetupUsernameRequest {
  username: string;
}

export interface UpdatePreferredCalendarRequest {
  preferredCalendar: string;
}

export interface ForgotPasswordRequest {
  identifier: string;
}

export interface ResetPasswordRequest {
  token: string;
  password: string;
}
export interface UpdateFullNameRequest {
  fullName: string;
}

export interface UpdateProfessionRequest {
  profession?: string;
}