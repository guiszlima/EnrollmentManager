import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { StudentService } from '../../../services/student.service';
import { StudentResponseDto } from '../../../models/student';

import { extractApiErrors } from '../../../utills/api-errors-utills';
import { ApiErrorsComponent } from '../../../components/shared/api-errors.component';

@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './students.component.html',
})
export class StudentsComponent implements OnInit {
  students: StudentResponseDto[] = [];
  filteredStudents: StudentResponseDto[] = [];
  filters = { name: '', email: '', registrationNumber: '', nationality: '', phone: '' };
  loading = false;
  errorMessage = '';
  errors: string[] = [];

  constructor(
    private studentService: StudentService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.load();
    console.log(this.students);
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    this.errors = [];

    this.studentService.getAll(this.filters).subscribe({
      next: (response) => {
        // O setTimeout com 0ms joga a execução para o final da fila,
        // garantindo que o Angular está pronto para atualizar o HTML.
        setTimeout(() => {
          this.students = (response.data ?? []).filter(
            (student) => student.userId != null && student.userId > 0,
          );

          this.filteredStudents = this.students;
          this.loading = false;

          // Força a renderização após os dados estarem prontos
          this.cdr.detectChanges();
        }, 0);
      },
      error: (error: unknown) => {
        setTimeout(() => {
          this.fail(error);
        }, 0);
      },
    });
  }

  filterStudents(): void {
    this.load();
  }

  private fail(error: unknown): void {
    this.errors = extractApiErrors(error);

    this.errorMessage =
      this.errors[0] ?? 'Não foi possível carregar os alunos.';

    this.loading = false;
    this.cdr.detectChanges();
  }
}
