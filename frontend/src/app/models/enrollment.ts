export interface EnrollmentCreateDto {
  studentId: number;
  courseId: number;
  formatId: number;
}

export interface EnrollmentStatusChangeDto {
  statusId: number;
}

export interface EnrollmentResponseDto {
  id: number;
  studentId: number;
  studentName: string;
  courseId: number;
  courseName: string;
  statusId: number;
  statusName: string;
  formatId: number;
  formatName: string;
  enrollmentDate: string;
  completionDate?: string | null;
}
