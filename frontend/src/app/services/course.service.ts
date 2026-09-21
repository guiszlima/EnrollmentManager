import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api-response';
import { CourseInputDto, CourseReportDto, CourseResponseDto } from '../models/course';

@Injectable({ providedIn: 'root' })
export class CourseService {
  private readonly apiUrl = `${environment.apiUrl}/api/courses`;

  constructor(private http: HttpClient) {}

  getAll(filter: Partial<{ courseTypeId: number; educationLevelId: number; statusId: number; formatIds: number[] }> = {}): Observable<ApiResponse<CourseResponseDto[]>> {
    let params = new HttpParams();
    if (filter.courseTypeId) params = params.set('courseTypeId', filter.courseTypeId);
    if (filter.educationLevelId) params = params.set('educationLevelId', filter.educationLevelId);
    if (filter.statusId) params = params.set('statusId', filter.statusId);
    for (const formatId of filter.formatIds ?? []) params = params.append('formatIds', formatId);
    return this.http.get<ApiResponse<CourseResponseDto[]>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<ApiResponse<CourseResponseDto>> {
    return this.http.get<ApiResponse<CourseResponseDto>>(`${this.apiUrl}/${id}`);
  }

  getReport(id: number, filter: { enrollmentStatusIds: number[]; formatIds: number[]; teacherIds: number[] } = { enrollmentStatusIds: [], formatIds: [], teacherIds: [] }): Observable<ApiResponse<CourseReportDto>> {
    let params = new HttpParams();
    for (const value of filter.enrollmentStatusIds) params = params.append('enrollmentStatusIds', value);
    for (const value of filter.formatIds) params = params.append('formatIds', value);
    for (const value of filter.teacherIds) params = params.append('teacherIds', value);
    return this.http.get<ApiResponse<CourseReportDto>>(`${this.apiUrl}/${id}/report`, { params });
  }

  create(payload: CourseInputDto): Observable<ApiResponse<CourseResponseDto>> {
    return this.http.post<ApiResponse<CourseResponseDto>>(this.apiUrl, payload);
  }

  update(id: number, payload: CourseInputDto): Observable<ApiResponse<CourseResponseDto>> {
    return this.http.put<ApiResponse<CourseResponseDto>>(`${this.apiUrl}/${id}`, payload);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
