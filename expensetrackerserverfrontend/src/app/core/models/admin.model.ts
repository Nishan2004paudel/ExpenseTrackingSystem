export interface AdminUser {
  userId: number;
  username?: string;
  email: string;
  fullName: string;
  profession?: string;
  preferredCalendar: string;
  role: string;
  authProvider: string;
  isEmailVerified: boolean;
  isActive: boolean;
  deactivatedBy?: number;
  deactivatedAt?: string;
  deactivationReason?: string;
  createdAt: string;
  updatedAt?: string;
}