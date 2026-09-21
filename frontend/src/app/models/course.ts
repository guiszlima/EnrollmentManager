export interface CourseInputDto {
  name: string;
  courseTypeId: number;
  educationLevelId: number;
  courseStatusId: number;
  formatIds: number[];
  teacherIds: number[];
  totalSlots: number;
}

export interface CourseResponseDto {
  id: number;
  name: string;
  courseTypeId: number;
  courseTypeName: string;
  educationLevelId: number;
  educationLevelName: string;
  statusId: number;
  statusName: string;
  formatIds: number[];
  teacherIds: number[];
  totalSlots: number;
  availableSlots: number;
  enrollmentCount: number;
  enrollments: CourseEnrollmentDto[];
}

export interface CourseEnrollmentDto {
  id: number;
  studentId: number;
  studentName: string;
  studentEmail: string;
  registrationNumber: string;
  formatId: number;
  formatName: string;
  statusId: number;
  statusName: string;
  enrollmentDate: string;
}

export interface CourseReportDto {
  course: CourseReportCourseDto;
  enrollments: CourseReportEnrollmentDto[];
  teachers: CourseReportTeacherDto[];
}

export interface CourseReportCourseDto {
  id: number;
  name: string;
  courseTypeName: string;
  educationLevelName: string;
  statusName: string;
  totalSlots: number;
  availableSlots: number;
  enrollmentCount: number;
  formats: CourseReportFormatDto[];
}

export interface CourseReportEnrollmentDto {
  id: number;
  enrollmentStatus: string;
  enrollmentDate: string;
  format: CourseReportFormatDto | null;
  student: CourseReportStudentDto;
}

export interface CourseReportStudentDto {
  userId: number;
  name: string;
  email: string;
  registrationNumber: string;
  nationality: string | null;
  phone: string;
}

export interface CourseReportTeacherDto {
  userId: number;
  name: string;
  email: string;
  formats: CourseReportFormatDto[];
}

export interface CourseReportFormatDto { id: number; name: string; }
