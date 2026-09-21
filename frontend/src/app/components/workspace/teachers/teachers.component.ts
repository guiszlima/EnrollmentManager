import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TeacherService } from '../../../services/teacher.service';
import { TeacherResponseDto } from '../../../models/teacher';
import { CatalogService } from '../../../services/catalog.service';
import { StudyFormatDto } from '../../../models/catalog';
import { ApiErrorsComponent } from '../../../components/shared/api-errors.component';
import { extractApiErrors } from '../../../utills/api-errors-utills';

@Component({
  selector: 'app-teachers',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './teachers.component.html',
})
export class TeachersComponent implements OnInit {
  teachers: TeacherResponseDto[] = [];
  formats: StudyFormatDto[] = [];
  filters = { name: '', email: '', studyFormatId: 0 };
  errors: string[] = [];
  loading = false;

  get configuredCount(): number { return this.teachers.filter(teacher => teacher.formatIds.length > 0).length; }

  constructor(
    private service: TeacherService,
    private catalog: CatalogService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.catalog.studyFormats().subscribe({
      next: response => {
        this.formats = response.data ?? [];
        this.cdr.markForCheck();
      },
      error: error => this.setErrors(error),
    });
    this.load();
  }

  load(): void {
    this.loading = true;
    this.service.getAll(this.filters).subscribe({
      next: response => {
        this.teachers = response.data ?? [];
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: error => this.setErrors(error),
    });
  }

  formatNames(teacher: TeacherResponseDto): string {
    return teacher.formatIds
      .map(id => this.formats.find(format => format.id === id)?.name)
      .filter(Boolean)
      .join(', ') || 'Nenhum formato informado';
  }

  applyFilters(): void { this.load(); }

  private setErrors(error: unknown): void {
    this.errors = extractApiErrors(error);
    this.loading = false;
    this.cdr.detectChanges();
  }
}
