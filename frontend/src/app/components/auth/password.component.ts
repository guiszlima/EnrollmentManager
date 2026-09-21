import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ApiErrorsComponent } from '../shared/api-errors.component';

@Component({
  selector: 'app-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './password.component.html',
})
export class PasswordComponent {
  reset = false;
  email = '';
  token = '';
  newPassword = '';
  loading = false;
  message = '';
  errorMessage = '';
  errors: string[] = [];

  constructor(
    private auth: AuthService,
    route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
  ) {
    this.reset = route.snapshot.data['reset'] === true;
  }

  submit(): void {
    this.loading = true;
    this.message = '';
    this.errorMessage = '';
    this.errors = [];
    const request = this.reset
      ? this.auth.resetPassword({ token: this.token, newPassword: this.newPassword })
      : this.auth.forgotPassword({ email: this.email });
    request.subscribe({
      next: (response) => {
        this.errors = response.errors;
        this.message = response.message ?? 'Operação realizada com sucesso.';
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errors = this.auth.getErrorMessages(error, 'Não foi possível concluir a operação.');
        this.errorMessage = this.errors[0];
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }
}
