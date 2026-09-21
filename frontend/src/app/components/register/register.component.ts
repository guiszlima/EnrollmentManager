import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ApiErrorsComponent } from '../shared/api-errors.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './register.component.html',
})
export class RegisterComponent {
  model = { userName: '', email: '', password: '' };
  loading = false;
  errorMessage = '';
  errors: string[] = [];
  successMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {}

  register(): void {
    if (this.model.password.length < 6) {
      this.errors = ['A senha deve ter no mínimo 6 caracteres.'];
      this.successMessage = '';
      return;
    }
    this.loading = true;
    this.errorMessage = '';
    this.errors = [];
    this.successMessage = '';
    this.authService.register(this.model).subscribe({
      next: () => {
        this.successMessage =
          'Cadastro realizado. Aguarde a aprovação de um administrador antes de entrar.';
        this.loading = false;
        setTimeout(
          () =>
            this.router.navigate(['/login'], {
              state: {
                registrationMessage:
                  'Registro no sistema solicitado, aguarde a confirmação do Administrador.',
              },
            }),
          800,
        );
      },
      error: (error) => {
        this.errors = this.authService.getErrorMessages(error, 'Não foi possível cadastrar.');
        this.errorMessage = this.errors[0];
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }
}
