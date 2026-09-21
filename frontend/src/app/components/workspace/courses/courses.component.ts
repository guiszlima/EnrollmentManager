import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { CourseService } from '../../../services/course.service';
import { CourseResponseDto } from '../../../models/course';
import { CourseStatusResponseDto, CourseTypeResponseDto, EducationLevelResponseDto, StudyFormatDto } from '../../../models/catalog';
import { CatalogService } from '../../../services/catalog.service';
import { extractApiErrors } from '../../../utills/api-errors-utills';
import { ApiErrorsComponent } from '../../../components/shared/api-errors.component';

@Component({
  selector: 'app-courses',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './courses.component.html'
})
export class CoursesComponent implements OnInit {
  courses: CourseResponseDto[] = [];
  searchTerm = '';
  statusFilter = 'all';
  loading = false;
  errors: string[] = [];
  courseTypes: CourseTypeResponseDto[] = [];
  educationLevels: EducationLevelResponseDto[] = [];
  statuses: CourseStatusResponseDto[] = [];
  formats: StudyFormatDto[] = [];
  filters = { courseTypeId: 0, educationLevelId: 0, statusId: 0, formatIds: [] as number[] };
  generatingReport = false;

  constructor(private courseService: CourseService, private catalog: CatalogService, private auth: AuthService, private cdr: ChangeDetectorRef) {}

  get filteredCourses(): CourseResponseDto[] {
    const query = this.searchTerm.trim().toLowerCase();
    return this.courses.filter(course => {
      const matchesSearch = !query || [course.name, course.courseTypeName, course.educationLevelName].some(value => value?.toLowerCase().includes(query));
      const isActive = course.statusName?.toLowerCase().includes('ativo') || course.statusName?.toLowerCase().includes('oferta');
      return matchesSearch && (this.statusFilter === 'all' || this.statusFilter === 'active' && isActive || this.statusFilter === 'other' && !isActive);
    });
  }

  get activeCourses(): number {
    return this.courses.filter(course => course.statusName?.toLowerCase().includes('ativo') || course.statusName?.toLowerCase().includes('oferta')).length;
  }

  ngOnInit(): void {
    this.catalog.courseTypes().subscribe({ next: response => this.courseTypes = response.data ?? [], error: error => this.setErrors(error) });
    this.catalog.educationLevels().subscribe({ next: response => this.educationLevels = response.data ?? [], error: error => this.setErrors(error) });
    this.catalog.courseStatuses().subscribe({ next: response => this.statuses = response.data ?? [], error: error => this.setErrors(error) });
    this.catalog.studyFormats().subscribe({ next: response => this.formats = response.data ?? [], error: error => this.setErrors(error) });
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errors = [];
    this.courseService.getAll(this.filters).subscribe({
          next: (response) => {
        this.courses = response.data ?? [];
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: error => {
        this.errors = extractApiErrors(error);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  applyFilters(): void { this.load(); }

  deleteCourse(id: number): void {
    if (!confirm('Deseja excluir este curso?')) return;
    this.courseService.delete(id).subscribe({ next: () => this.load(), error: error => this.setErrors(error) });
  }

  generateGeneralReport(): void {
    this.generatingReport = true;
    this.courseService.getAll().subscribe({
      next: response => {
        const popup = window.open('', '_blank', 'width=960,height=720');
        if (!popup) {
          this.errors = ['Permita pop-ups para abrir o relatório geral.'];
          this.generatingReport = false;
          return;
        }

        popup.document.open();
        popup.document.write(this.buildGeneralReport(response.data ?? []));
        popup.document.close();
        popup.focus();
        window.setTimeout(() => popup.print(), 300);
        this.generatingReport = false;
      },
      error: error => {
        this.setErrors(error);
        this.generatingReport = false;
      },
    });
  }

  private buildGeneralReport(courses: CourseResponseDto[]): string {
    const rows = courses.map(course => `
      <article class="course">
        <h2>${this.escapeHtml(course.name)}</h2>
        <div class="details">
          <span><b>Tipo:</b> ${this.escapeHtml(course.courseTypeName)}</span>
          <span><b>Nível:</b> ${this.escapeHtml(course.educationLevelName)}</span>
          <span><b>Status:</b> ${this.escapeHtml(course.statusName)}</span>
          <span><b>Vagas disponíveis:</b> ${course.availableSlots}</span>
          <span><b>Total de vagas:</b> ${course.totalSlots}</span>
          <span><b>Matrículas:</b> ${course.enrollmentCount}</span>
        </div>
      </article>`).join('');

    return `<!doctype html><html lang="pt-BR"><head><meta charset="utf-8"><title>Relatório Geral de Cursos</title><style>
      @page{size:A4;margin:18mm}*{box-sizing:border-box}body{margin:0;color:#17261f;font-family:Arial,sans-serif;font-size:12px}header{border-bottom:2px solid #1f5b49;padding-bottom:16px;margin-bottom:22px}h1{font-size:24px;margin:0 0 6px}p{color:#64748b;margin:0}.course{border:1px solid #dbe4de;border-radius:10px;padding:14px 16px;margin:0 0 12px;break-inside:avoid}.course h2{font-size:16px;margin:0 0 10px;color:#1f5b49}.details{display:grid;grid-template-columns:repeat(3,1fr);gap:8px;color:#475569}.details b{color:#17261f}@media print{.course{box-shadow:none}}
    </style></head><body><header><h1>RELATÓRIO GERAL DE CURSOS</h1><p>Documento administrativo · ${new Date().toLocaleDateString('pt-BR')}</p></header>${rows || '<p>Nenhum curso cadastrado.</p>'}</body></html>`;
  }

  private escapeHtml(value: string | null | undefined): string {
    return String(value ?? '').replace(/[&<>'"]/g, character => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', "'": '&#39;', '"': '&quot;' }[character] ?? character));
  }

  canManage(): boolean {
    return this.auth.hasAnyRole(['Admin', 'Secretary']);
  }

  canEdit(): boolean {
    return this.auth.hasRole('Admin');
  }

  private setErrors(error: unknown): void {
    this.errors = extractApiErrors(error);
    this.cdr.detectChanges();
  }
}
