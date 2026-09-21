import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { apiEndpoints } from '../api/api-endpoints';
import { ApiResponse } from '../models/api-response';
import { EnrollmentCreateDto, EnrollmentResponseDto, EnrollmentStatusChangeDto } from '../models/enrollment';

@Injectable({ providedIn: 'root' })
export class EnrollmentService {
  private readonly url = `${environment.apiUrl}${apiEndpoints.enrollments}`;
  constructor(private http: HttpClient) {}
  getById(id: number): Observable<ApiResponse<EnrollmentResponseDto>> { return this.http.get<ApiResponse<EnrollmentResponseDto>>(`${this.url}/${id}`); }
  getAll(): Observable<ApiResponse<EnrollmentResponseDto[]>> { return this.http.get<ApiResponse<EnrollmentResponseDto[]>>(this.url); }
  getByStudent(id: number): Observable<ApiResponse<EnrollmentResponseDto[]>> { return this.http.get<ApiResponse<EnrollmentResponseDto[]>>(`${this.url}/student/${id}`); }
  create(payload: EnrollmentCreateDto): Observable<ApiResponse<EnrollmentResponseDto>> { return this.http.post<ApiResponse<EnrollmentResponseDto>>(this.url, payload); }
  changeStatus(id: number, payload: EnrollmentStatusChangeDto): Observable<ApiResponse<EnrollmentResponseDto>> { return this.http.patch<ApiResponse<EnrollmentResponseDto>>(`${this.url}/${id}/status`, payload); }
}
