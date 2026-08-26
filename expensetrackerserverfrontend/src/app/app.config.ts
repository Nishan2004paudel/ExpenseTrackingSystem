import { ApplicationConfig, inject } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAppInitializer } from '@angular/core';
import { routes } from './app.routes';
import { AuthService } from './core/services/auth.service';
import { ProfileService } from './core/services/profile.service';
import { catchError, of } from 'rxjs';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { NotificationService } from './core/services/notification.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAppInitializer(() => {
      const auth = inject(AuthService);
      const profile = inject(ProfileService);
      const notifications = inject(NotificationService);

      return new Promise<void>((resolve) => {
        auth.refresh().pipe(
          catchError(() => of(null))
        ).subscribe(() => {
          if (auth.isAuthenticated()) {
            profile.getMe().pipe(
              catchError(() => of(null))
            ).subscribe(() => {
              notifications.startConnection();
              resolve();
            });
          } else {
            resolve();
          }
        });
      });
    })
  ]
};