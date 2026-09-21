import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CatalogService } from '../../services/catalog.service';
import { TeacherService } from '../../services/teacher.service';
import { StudyFormatDto } from '../../models/catalog';
import { extractApiErrors } from '../../utills/api-errors-utills';
import { ApiErrorsComponent } from '../shared/api-errors.component';

@Component({
  selector: 'app-teacher-edit',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './teacher-edit.component.html',
})
export class TeacherEditComponent implements OnInit {
  id = 0;
  userName = '';
  email = '';
  formatIds: number[] = [];
  formats: StudyFormatDto[] = [];
  errors: string[] = [];
  loading = false;

  constructor(private route: ActivatedRoute, private router: Router, private service: TeacherService, private catalog: CatalogService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.catalog.studyFormats().subscribe({ next: response => { this.formats = response.data ?? []; this.cdr.markForCheck(); }, error: error => this.fail(error) });
    this.service.getAll().subscribe({
      next: response => {
        const teacher = (response.data ?? []).find(item => item.userId === this.id);
        if (teacher) { this.userName = teacher.userName; this.email = teacher.email; this.formatIds = [...teacher.formatIds]; }
        this.cdr.markForCheck();
      },
      error: error => this.fail(error),
    });
  }

  save(): void {
    this.formatIds = this.formatIds.map(Number).filter(id => id > 0);
    if (this.formatIds.length === 0) {
      this.errors = ['Selecione ao menos um formato de estudo.'];
      return;
    }
    this.loading = true;
    this.service.update(this.id, this.formatIds).subscribe({ next: () => this.router.navigate(['/teachers']), error: error => this.fail(error) });
  }

  private fail(error: unknown): void { this.errors = extractApiErrors(error); this.loading = false; this.cdr.detectChanges(); }
}
