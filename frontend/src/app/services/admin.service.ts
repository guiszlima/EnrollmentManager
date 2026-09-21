import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { apiEndpoints } from '../api/api-endpoints';
import { ApiResponse } from '../models/api-response';
import { AdminUserDto, ApproveUserDto, ChangeUserRoleDto } from '../models/admin';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly url = `${environment.apiUrl}${apiEndpoints.adminUsers}`;
  constructor(private http: HttpClient) {}
  users(active = true): Observable<ApiResponse<AdminUserDto[]>> { return this.http.get<ApiResponse<AdminUserDto[]>>(this.url, { params: { active } }); }
  approve(id: number, payload: ApproveUserDto): Observable<ApiResponse<AdminUserDto>> { return this.http.patch<ApiResponse<AdminUserDto>>(`${this.url}/${id}/approve`, payload); }
  changeRole(id: number, payload: ChangeUserRoleDto): Observable<ApiResponse<AdminUserDto>> { return this.http.patch<ApiResponse<AdminUserDto>>(`${this.url}/${id}/role`, payload); }
  remove(id: number): Observable<ApiResponse<boolean>> { return this.http.delete<ApiResponse<boolean>>(`${this.url}/${id}`); }
  resetPassword(id: number): Observable<ApiResponse<boolean>> { return this.http.post<ApiResponse<boolean>>(`${this.url}/${id}/reset-password`, {}); }
}
