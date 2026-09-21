import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const token = auth.getToken();
  const isAuthRequest = request.url.toLowerCase().includes('/api/auth/');
  const requestWithToken = token && !isAuthRequest
    ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : request;

  return next(requestWithToken).pipe(
    catchError(error => {
      if (error.status === 401 && !isAuthRequest) {
        auth.logout();
        void router.navigate(['/login'], { queryParams: { reason: 'session-expired' } });
      }

      return throwError(() => error);
    })
  );
};
