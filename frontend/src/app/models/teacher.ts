export interface UserTeacherCreateDto {
  userName: string;
  email: string;
  password: string;
  formatIds: number[];
}

export interface TeacherResponseDto {
  userId: number;
  userName: string;
  email: string;
  formatIds: number[];
}
