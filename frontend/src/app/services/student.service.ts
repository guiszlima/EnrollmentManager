import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { apiEndpoints } from '../api/api-endpoints';
import { ApiResponse } from '../models/api-response';
import { StudentCreateDto, StudentResponseDto, StudentUpdateDto, UserStudentCreateDto } from '../models/student';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private readonly url = `${environment.apiUrl}${apiEndpoints.students}`;
  constructor(private http: HttpClient) {}
  getAll(filter: Partial<{ name: string; email: string; registrationNumber: string; nationality: string; phone: string }> = {}): Observable<ApiResponse<StudentResponseDto[]>> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(filter)) if (value?.trim()) params = params.set(key, value.trim());
    return this.http.get<ApiResponse<StudentResponseDto[]>>(this.url, { params });
  }
  getById(id: number): Observable<ApiResponse<StudentResponseDto>> { return this.http.get<ApiResponse<StudentResponseDto>>(`${this.url}/${id}`); }
  create(payload: StudentCreateDto): Observable<ApiResponse<StudentResponseDto>> { return this.http.post<ApiResponse<StudentResponseDto>>(this.url, payload); }
  createWithUser(payload: UserStudentCreateDto): Observable<ApiResponse<StudentResponseDto>> { return this.http.post<ApiResponse<StudentResponseDto>>(`${this.url}/with-user`, payload); }
  update(id: number, payload: StudentUpdateDto): Observable<ApiResponse<StudentResponseDto>> { return this.http.put<ApiResponse<StudentResponseDto>>(`${this.url}/${id}`, payload); }
  remove(id: number): Observable<ApiResponse<boolean>> { return this.http.delete<ApiResponse<boolean>>(`${this.url}/${id}`); }
}
