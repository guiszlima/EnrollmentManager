import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { apiEndpoints } from '../api/api-endpoints';
import { ApiResponse } from '../models/api-response';
import { RoleResponseDto } from '../models/admin';

import {
  CourseStatusResponseDto,
  StudyFormatDto,
  CourseTypeResponseDto,
  EducationLevelResponseDto,
  EnrollmentStatusResponseDto,
  CourseStudyFormatDto,
} from '../models/catalog';

@Injectable({ providedIn: 'root' })
export class CatalogService {
  private readonly base = environment.apiUrl;

  constructor(private http: HttpClient) {}

  courseTypes(): Observable<ApiResponse<CourseTypeResponseDto[]>> {
    return this.http.get<ApiResponse<CourseTypeResponseDto[]>>(
      `${this.base}${apiEndpoints.courseTypes}`,
    );
  }

  educationLevels(): Observable<ApiResponse<EducationLevelResponseDto[]>> {
    return this.http.get<ApiResponse<EducationLevelResponseDto[]>>(
      `${this.base}${apiEndpoints.educationLevels}`,
    );
  }

  courseStatuses(): Observable<ApiResponse<CourseStatusResponseDto[]>> {
    return this.http.get<ApiResponse<CourseStatusResponseDto[]>>(
      `${this.base}${apiEndpoints.courseStatuses}`,
    );
  }

  enrollmentStatuses(): Observable<ApiResponse<EnrollmentStatusResponseDto[]>> {
    return this.http.get<ApiResponse<EnrollmentStatusResponseDto[]>>(
      `${this.base}${apiEndpoints.enrollmentStatuses}`,
    );
  }

  courseStudyFormats(): Observable<ApiResponse<CourseStudyFormatDto[]>> {
    return this.http.get<ApiResponse<CourseStudyFormatDto[]>>(
      `${this.base}${apiEndpoints.courseStudyFormats}`,
    );
  }

  studyFormats(): Observable<ApiResponse<StudyFormatDto[]>> {
    return this.http.get<ApiResponse<StudyFormatDto[]>>(
      `${this.base}${apiEndpoints.studyFormats}`,
    );
  }

  roles(): Observable<ApiResponse<RoleResponseDto[]>> {
    return this.http.get<ApiResponse<RoleResponseDto[]>>(
      `${this.base}${apiEndpoints.roles}`,
    );
  }
}
