import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { CatalogService } from '../../services/catalog.service';
import { TeacherService } from '../../services/teacher.service';
import { StudyFormatDto } from '../../models/catalog';
import { extractApiErrors } from '../../utills/api-errors-utills';
import { ApiErrorsComponent } from '../shared/api-errors.component';

@Component({
  selector: 'app-teacher-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './teacher-form.component.html',
})
export class TeacherFormComponent {
  model = { userName: '', email: '', password: '', formatIds: [] as number[] };
  formats: StudyFormatDto[] = [];
  errors: string[] = [];
  loading = false;

  constructor(
    private catalog: CatalogService,
    private service: TeacherService,
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {
    this.catalog.studyFormats().subscribe({
      next: response => {
        this.formats = response.data ?? [];
        this.cdr.markForCheck();
      },
      error: error => this.setErrors(error),
    });
  }

  save(): void {
    this.model.formatIds = this.model.formatIds.map(Number).filter(id => id > 0);
    if (this.model.formatIds.length === 0) {
      this.errors = ['Selecione ao menos um formato de estudo.'];
      return;
    }
    this.loading = true;
    this.errors = [];
    this.service.createWithUser(this.model).subscribe({
      next: () => this.router.navigate(['/courses']),
      error: error => {
        this.setErrors(error);
        this.loading = false;
      },
    });
  }

  private setErrors(error: unknown): void {
    this.errors = extractApiErrors(error);
    this.loading = false;
    this.cdr.detectChanges();
  }
}
