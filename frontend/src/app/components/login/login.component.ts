import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ApiErrorsComponent } from '../shared/api-errors.component';
import { extractApiErrors } from '../../utills/api-errors-utills';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  model = { email: '', password: '' };
  loading = false;
  errors: string[] = [];
  successMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {
    this.successMessage = history.state?.['registrationMessage'] ?? '';
  }

  login(): void {
    this.loading = true;
    this.errors = [];
    this.successMessage = '';
    this.authService.login(this.model).subscribe({
      next: (response) => {
        const token = response.data;
        if (token) {
          this.router.navigate(['/dashboard']);
          return;
        }
        this.errors =
          response.errors?.length > 0 ? response.errors : [response.message ?? 'Falha no login.'];
        this.loading = false;
      },
      error: (error) => {
        this.errors = extractApiErrors(error);
        this.successMessage = '';
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }
}
