export interface CourseTypeResponseDto {
  id: number;
  name: string;
}

export interface EducationLevelResponseDto {
  id: number;
  name: string;
}

export interface CourseStatusResponseDto {
  id: number;
  name: string;
  code: string;
  description?: string | null;
}

export interface EnrollmentStatusResponseDto {
  id: number;
  name: string;
  code: string;
}

export interface CourseStudyFormatDto {
  courseId: number;
  courseName: string;
  formatId: number;
  formatName: string;
}

export interface StudyFormatDto {
  id: number;
  name: string;
}

export interface CourseInputDto {
  name: string;
  courseTypeId: number;
  educationLevelId: number;
  courseStatusId: number;
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
}
