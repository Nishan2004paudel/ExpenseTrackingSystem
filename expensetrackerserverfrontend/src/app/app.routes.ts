import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'check-email',
    loadComponent: () =>
      import('./features/auth/check-email/check-email.component').then(m => m.CheckEmailComponent)
  },
  {
    path: 'forgot-password',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent)
  },
  {
    path: 'reset-password',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent)
  },
  {
    path: 'verify-email',
    loadComponent: () =>
      import('./features/auth/verify-email/verify-email.component').then(m => m.VerifyEmailComponent)
  },
  {
    path: 'terms-of-service',
    loadComponent: () =>
      import('./features/auth/terms/terms.component').then(m => m.TermsComponent)
  },
  {
    path: 'privacy-policy',
    loadComponent: () =>
      import('./features/auth/privacy/privacy.component').then(m => m.PrivacyComponent)
  },
  {
    path: 'verify-email-change',
    loadComponent: () =>
      import('./features/settings/verify-email-change/verify-email-change.component').then(m => m.VerifyEmailChangeComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/layout/layout.component').then(m => m.LayoutComponent),
    children: [
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/profile/profile.component').then(m => m.ProfileComponent)
      },
      {
        path: 'categories',
        loadComponent: () =>
          import('./features/category/category.component').then(m => m.CategoryComponent)
      },
      {
        path: 'expenses',
        loadComponent: () =>
          import('./features/expense/expense.component').then(m => m.ExpenseComponent)
      },
      {
        path: 'budgets',
        loadComponent: () =>
          import('./features/budget/budget.component').then(m => m.BudgetComponent)
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./features/settings/settings.component').then(m => m.SettingsComponent)
      },
      {
        path: 'admin',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/admin/admin.component').then(m => m.AdminComponent)
      }
    ]
  }
];