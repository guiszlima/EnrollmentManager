import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CourseService } from '../../../services/course.service';
import { EnrollmentService } from '../../../services/enrollment.service';
import { StudentService } from '../../../services/student.service';
import { AuthService } from '../../../services/auth.service';
import { CourseResponseDto } from '../../../models/course';
import { EnrollmentResponseDto } from '../../../models/enrollment';
import { StudentResponseDto } from '../../../models/student';
import { extractApiErrors } from '../../../utills/api-errors-utills';
import { ApiErrorsComponent } from '../../../components/shared/api-errors.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, ApiErrorsComponent],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  courses: CourseResponseDto[] = [];
  students: StudentResponseDto[] = [];
  enrollments: EnrollmentResponseDto[] = [];
  loading = true;
  studentsAvailable = false;
  errorMessage = '';
  errors: string[] = [];

  constructor(
    private courseService: CourseService,
    private studentService: StudentService,
    private enrollmentService: EnrollmentService,
    private auth: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    this.errors = [];
    this.studentsAvailable = this.isStaff;
    let pending = 3;
    const complete = (): void => {
      pending -= 1;
      if (pending === 0) {
        this.loading = false;
        this.cdr.markForCheck();
      }
    };

    this.courseService.getAll().subscribe({
      next: response => {
        this.courses = response.data ?? [];
        complete();
      },
      error: error => this.fail(error)
    });

    this.studentService.getAll().subscribe({
      next: response => { this.students = response.data ?? []; complete(); },
      error: error => this.fail(error)
    });

    this.enrollmentService.getAll().subscribe({
      next: response => { this.enrollments = response.data ?? []; complete(); },
      error: error => this.fail(error)
    });
  }

  get isStaff(): boolean {
    return this.auth.hasAnyRole(['Admin', 'Secretary']);
  }

  private fail(error: unknown): void {
    this.errors = extractApiErrors(error);
    this.errorMessage = this.errors[0] ?? 'Não foi possível carregar o dashboard.';
    this.loading = false;
    this.cdr.detectChanges();
  }
}
