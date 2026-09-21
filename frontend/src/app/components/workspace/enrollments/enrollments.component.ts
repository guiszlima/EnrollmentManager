import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { CatalogService } from '../../../services/catalog.service';
import { EnrollmentService } from '../../../services/enrollment.service';
import { AuthService } from '../../../services/auth.service';

import { EnrollmentResponseDto } from '../../../models/enrollment';
import { EnrollmentStatusResponseDto } from '../../../models/catalog';

import { extractApiErrors } from '../../../utills/api-errors-utills';
import { ApiErrorsComponent } from '../../../components/shared/api-errors.component';

@Component({
  selector: 'app-enrollments',
  standalone: true,
  imports: [CommonModule, FormsModule, ApiErrorsComponent],
  templateUrl: './enrollments.component.html',
})
export class EnrollmentsComponent implements OnInit {
  enrollments: EnrollmentResponseDto[] = [];

  filteredEnrollments: EnrollmentResponseDto[] = [];

  statuses: EnrollmentStatusResponseDto[] = [];

  query = '';

  loading = false;

  errorMessage = '';

  successMessage = '';

  errors: string[] = [];

  constructor(
    private enrollmentService: EnrollmentService,
    private catalog: CatalogService,
    private auth: AuthService,
    private cdr: ChangeDetectorRef,
  ) {}

  get isStaff(): boolean {
    return this.auth.hasAnyRole(['Admin', 'Secretary']);
  }

  ngOnInit(): void {
    this.catalog.enrollmentStatuses().subscribe({
      next: (response) => {
        this.statuses = response.data ?? [];

        this.cdr.markForCheck();
      },
      error: (error: unknown) => this.fail(error),
    });

    this.load();
  }

  load(): void {
    const userId = this.auth.getUserId();

    if (!userId) {
      this.loading = false;
      this.cdr.markForCheck();
      return;
    }

    this.loading = true;

    const request = this.isStaff
      ? this.enrollmentService.getAll()
      : this.enrollmentService.getByStudent(userId);

    request.subscribe({
      next: (response) => {
        this.enrollments = response.data ?? [];

        this.filterEnrollments();

        this.loading = false;

        this.cdr.markForCheck();
      },
      error: (error: unknown) => this.fail(error),
    });
  }

  filterEnrollments(): void {
    const query = this.query.trim().toLowerCase();

    this.filteredEnrollments = this.enrollments.filter(
      (enrollment) =>
        `${enrollment.studentName ?? ''} ${enrollment.courseName ?? ''} ${enrollment.statusName ?? ''}`
          .toLowerCase()
          .includes(query),
    );

    this.cdr.markForCheck();
  }

  availableStatuses(
    enrollment: EnrollmentResponseDto,
  ): EnrollmentStatusResponseDto[] {
    const current =
      this.statuses.find((status) => status.id === enrollment.statusId)
        ?.code ?? '';

    const transitions: Record<string, string[]> = {
      PENDING: ['APPROVED', 'CANCELLED'],
      APPROVED: ['SUSPENDED', 'COMPLETED', 'CANCELLED'],
      SUSPENDED: ['APPROVED', 'CANCELLED'],
    };

    return this.statuses.filter((status) =>
      transitions[current]?.includes(status.code),
    );
  }

  changeStatus(
    enrollment: EnrollmentResponseDto,
    statusId: number,
  ): void {
    this.successMessage = '';

    this.enrollmentService
      .changeStatus(enrollment.id, { statusId })
      .subscribe({
        next: (response) => {
          if (response.data) {
            this.enrollments = this.enrollments.map((item) =>
              item.id === enrollment.id ? response.data! : item,
            );

            this.filterEnrollments();
          }

          this.successMessage =
            response.message ??
            'Status da matrícula atualizado com sucesso.';

          this.cdr.markForCheck();
        },
        error: (error: unknown) => this.fail(error),
      });
  }

  private fail(error: unknown): void {
    this.errors = extractApiErrors(error);

    this.errorMessage =
      this.errors[0] ?? 'Não foi possível carregar as matrículas.';

    this.successMessage = '';
    this.loading = false;

    this.cdr.markForCheck();
  }
}
