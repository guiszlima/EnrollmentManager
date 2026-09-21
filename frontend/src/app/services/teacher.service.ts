import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { apiEndpoints } from '../api/api-endpoints';
import { ApiResponse } from '../models/api-response';
import { TeacherResponseDto, UserTeacherCreateDto } from '../models/teacher';

@Injectable({ providedIn: 'root' })
export class TeacherService {
  private readonly url = `${environment.apiUrl}${apiEndpoints.teachers}`;

  constructor(private http: HttpClient) {}

  getAll(filter: Partial<{ name: string; email: string; studyFormatId: number }> = {}): Observable<ApiResponse<TeacherResponseDto[]>> {
    let params = new HttpParams();
    if (filter.name?.trim()) params = params.set('name', filter.name.trim());
    if (filter.email?.trim()) params = params.set('email', filter.email.trim());
    if (filter.studyFormatId) params = params.set('studyFormatId', filter.studyFormatId);
    return this.http.get<ApiResponse<TeacherResponseDto[]>>(this.url, { params });
  }

  createWithUser(payload: UserTeacherCreateDto): Observable<ApiResponse<TeacherResponseDto>> {
    return this.http.post<ApiResponse<TeacherResponseDto>>(`${this.url}/with-user`, payload);
  }

  update(userId: number, formatIds: number[]): Observable<ApiResponse<TeacherResponseDto>> {
    return this.http.put<ApiResponse<TeacherResponseDto>>(`${this.url}/${userId}`, { formatIds });
  }
}
