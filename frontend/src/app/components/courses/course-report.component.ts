import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CourseService } from '../../services/course.service';
import { CourseReportDto } from '../../models/course';
import { EnrollmentStatusResponseDto } from '../../models/catalog';
import { CatalogService } from '../../services/catalog.service';
import { ApiErrorsComponent } from '../shared/api-errors.component';
import { extractApiErrors } from '../../utills/api-errors-utills';

@Component({
  selector: 'app-course-report',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './course-report.component.html',
})
export class CourseReportComponent implements OnInit {
  report: CourseReportDto | null = null;
  statuses: EnrollmentStatusResponseDto[] = [];
  filters = { enrollmentStatusIds: [] as number[], formatIds: [] as number[], teacherIds: [] as number[] };
  errors: string[] = [];
  loading = true;
  courseId = 0;

  constructor(private route: ActivatedRoute, private service: CourseService, private catalog: CatalogService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.courseId = Number(this.route.snapshot.paramMap.get('id'));
    this.catalog.enrollmentStatuses().subscribe({ next: response => { this.statuses = response.data ?? []; this.cdr.markForCheck(); }, error: error => this.setErrors(error) });
    this.load();
  }

  get formats() { return this.report?.course.formats ?? []; }
  get teachers() { return this.report?.teachers ?? []; }

  formatNames(teacher: { formats: { name: string }[] }): string {
    return teacher.formats.map(format => format.name).join(', ') || 'Nenhum formato informado';
  }

  load(): void {
    this.loading = true;
    this.service.getReport(this.courseId, this.filters).subscribe({
      next: response => { this.report = response.data; this.loading = false; this.cdr.markForCheck(); },
      error: error => this.setErrors(error),
    });
  }

  applyFilters(): void { this.load(); }

  private setErrors(error: unknown): void { this.errors = extractApiErrors(error); this.loading = false; this.cdr.detectChanges(); }
}
