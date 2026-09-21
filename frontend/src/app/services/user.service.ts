import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { apiEndpoints } from '../api/api-endpoints';
import { ApiResponse } from '../models/api-response';
import { UserResponseDto } from '../models/user';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly url = `${environment.apiUrl}${apiEndpoints.users}`;

  constructor(private http: HttpClient) {}

  getAvailableForStudent(): Observable<ApiResponse<UserResponseDto[]>> {
    return this.http.get<ApiResponse<UserResponseDto[]>>(`${this.url}/available-for-student`);
  }
}
