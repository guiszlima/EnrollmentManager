import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CourseService } from '../../services/course.service';
import { CourseResponseDto } from '../../models/course';
import { StudentService } from '../../services/student.service';
import { EnrollmentService } from '../../services/enrollment.service';
import { CatalogService } from '../../services/catalog.service';
import { StudentResponseDto } from '../../models/student';
import { StudyFormatDto } from '../../models/catalog';
import { AuthService } from '../../services/auth.service';
import { ApiErrorsComponent } from '../shared/api-errors.component';
import { extractApiErrors } from '../../utills/api-errors-utills';

@Component({ selector: 'app-course-details', standalone: true, imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent], templateUrl: './course-details.component.html' })
export class CourseDetailsComponent implements OnInit {
  course: CourseResponseDto | null = null;
  errors: string[] = [];
  loading = true;
  students: StudentResponseDto[] = [];
  formats: StudyFormatDto[] = [];
  studentId = 0;
  formatId = 0;
  savingEnrollment = false;

  constructor(private route: ActivatedRoute, private service: CourseService, private studentsService: StudentService, private enrollmentService: EnrollmentService, private catalog: CatalogService, public auth: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.catalog.studyFormats().subscribe({ next: response => { this.formats = response.data ?? []; this.cdr.markForCheck(); }, error: error => this.fail(error) });
    if (this.auth.hasAnyRole(['Admin', 'Secretary'])) {
      this.studentsService.getAll().subscribe({ next: response => { this.students = response.data ?? []; this.cdr.markForCheck(); }, error: error => this.fail(error) });
    }
    this.loadCourse(id);
  }

  get courseFormats(): StudyFormatDto[] {
    return this.formats.filter(format => this.course?.formatIds.includes(format.id));
  }

  addEnrollment(): void {
    if (!this.course || !this.studentId || !this.formatId) return;
    this.savingEnrollment = true;
    this.enrollmentService.create({ studentId: this.studentId, courseId: this.course.id, formatId: this.formatId }).subscribe({
      next: () => { const courseId = this.course!.id; this.studentId = 0; this.formatId = 0; this.savingEnrollment = false; this.loadCourse(courseId); },
      error: error => this.fail(error),
    });
  }

  private loadCourse(id: number): void {
    this.service.getById(id).subscribe({
      next: response => { this.course = response.data; this.loading = false; this.cdr.markForCheck(); },
      error: error => this.fail(error),
    });
  }

  private fail(error: unknown): void { this.errors = extractApiErrors(error); this.loading = false; this.savingEnrollment = false; this.cdr.detectChanges(); }
}
