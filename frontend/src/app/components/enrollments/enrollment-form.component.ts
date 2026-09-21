import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { CatalogService } from '../../services/catalog.service';
import { EnrollmentService } from '../../services/enrollment.service';
import { StudentService } from '../../services/student.service';
import { CourseService } from '../../services/course.service';
import { AuthService } from '../../services/auth.service';

import { CourseResponseDto } from '../../models/course';
import { CourseStudyFormatDto } from '../../models/catalog';
import { EnrollmentCreateDto } from '../../models/enrollment';
import { StudentResponseDto } from '../../models/student';

import { extractApiErrors } from '../../utills/api-errors-utills';
import { ApiErrorsComponent } from '../shared/api-errors.component';

@Component({
  selector: 'app-enrollment-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './enrollment-form.component.html',
})
export class EnrollmentFormComponent implements OnInit {
  model: EnrollmentCreateDto = {
    studentId: 0,
    courseId: 0,
    formatId: 0,
  };

  students: StudentResponseDto[] = [];

  courses: CourseResponseDto[] = [];

  formats: CourseStudyFormatDto[] = [];

  formatsForCourse: CourseStudyFormatDto[] = [];

  isStaff = false;

  loading = false;

  errors: string[] = [];

  constructor(
    private studentsService: StudentService,
    private coursesService: CourseService,
    private catalog: CatalogService,
    private service: EnrollmentService,
    private auth: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.isStaff = this.auth.hasAnyRole(['Admin', 'Secretary']);

    const userId = this.auth.getUserId();

    if (this.isStaff && this.auth.hasRole('Admin')) {
      this.studentsService.getAll().subscribe({
        next: (response) => this.setStudents(response.data ?? []),
        error: (error) => this.setErrors(error),
      });
    } else if (userId) {
      this.studentsService.getById(userId).subscribe({
        next: (response) =>
          this.setStudents(response.data ? [response.data] : []),
        error: (error) => this.setErrors(error),
      });
    }

    this.coursesService.getAll().subscribe({
      next: (response) => {
        this.courses = response.data ?? [];
        this.cdr.markForCheck();
      },
      error: (error) => this.setErrors(error),
    });

    this.catalog.courseStudyFormats().subscribe({
      next: (response) => {
        this.formats = response.data ?? [];
        this.refreshFormats();
        this.cdr.markForCheck();
      },
      error: (error) => this.setErrors(error),
    });
  }

  loadFormats(): void {
    this.refreshFormats();
    this.model.formatId = 0;
  }

  private refreshFormats(): void {
    const student = this.students.find(
      (item) => item.userId === this.model.studentId,
    );

    const studentFormatIds = new Set(student?.formatIds ?? []);

    this.formatsForCourse = this.formats.filter(
      (format) =>
        format.courseId === this.model.courseId &&
        studentFormatIds.has(format.formatId),
    );
  }

  save(): void {
    this.loading = true;
    this.errors = [];

    this.service.create(this.model).subscribe({
      next: () => this.router.navigate(['/enrollments']),
      error: (error) => {
        this.errors = extractApiErrors(error);
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  private setErrors(error: unknown): void {
    this.errors = extractApiErrors(error);
    this.cdr.detectChanges();
  }

  private setStudents(students: StudentResponseDto[]): void {
    this.students = students;

    if (!this.isStaff && this.students.length === 1) {
      this.model.studentId = this.students[0].userId;
    }

    this.refreshFormats();
    this.cdr.markForCheck();
  }
}
