import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable, Subject, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { apiEndpoints } from '../api/api-endpoints';
import { ApiResponse } from '../models/api-response';
import { ForgotPasswordRequest, LoginRequest, RegisterRequest, ResetPasswordRequest } from '../models/auth';
import { extractApiErrors } from '../utills/api-errors-utills';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}${apiEndpoints.auth}`;
  private readonly tokenKey = 'enrollment_token';
  private readonly sessionChangedSubject = new Subject<void>();
  readonly sessionChanged$ = this.sessionChangedSubject.asObservable();

  constructor(private http: HttpClient) {}

  register(payload: RegisterRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/register`, {
      userName: payload.userName.trim(),
      email: payload.email.trim().toLowerCase(),
      password: payload.password
    });
  }

  login(payload: LoginRequest): Observable<ApiResponse<string>> {
  return this.http.post<ApiResponse<string>>(
    `${this.apiUrl}/login`,
    {
      email: payload.email.trim().toLowerCase(),
      password: payload.password
    }
  ).pipe(
    tap((response) => {
      const token = response.data;

      if (token) {
        localStorage.setItem(this.tokenKey, token);
      }
    })
  );
}

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.sessionChangedSubject.next();
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  hasRole(role: string): boolean {
    return this.getRoles().some(currentRole => currentRole.toLowerCase() === role.toLowerCase());
  }

  hasAnyRole(roles: string[]): boolean {
    return roles.some(role => this.hasRole(role));
  }

  getUserId(): number | null {
    const payload = this.getTokenPayload();
    const claim = payload?.['nameid'] ?? payload?.['sub'] ?? payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    const userId = Number(claim);
    return Number.isInteger(userId) && userId > 0 ? userId : null;
  }

  getRoles(): string[] {
    const payload = this.getTokenPayload();
    const claim = payload?.['role'] ?? payload?.['roles'] ?? payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    return Array.isArray(claim) ? claim : typeof claim === 'string' ? [claim] : [];
  }

  private getTokenPayload(): Record<string, unknown> | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const encodedPayload = token.split('.')[1];
      const normalizedPayload = encodedPayload.replace(/-/g, '+').replace(/_/g, '/');
      return JSON.parse(atob(normalizedPayload)) as Record<string, unknown>;
    } catch {
      return null;
    }
  }

  getErrorMessage(error: HttpErrorResponse, fallback: string): string {
    return extractApiErrors(error, fallback)[0];
  }

  getErrorMessages(error: HttpErrorResponse, fallback: string): string[] {
    return extractApiErrors(error, fallback);
  }

  forgotPassword(payload: ForgotPasswordRequest): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/forgot-password`, payload);
  }

  resetPassword(payload: ResetPasswordRequest): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/reset-password`, payload);
  }
}
