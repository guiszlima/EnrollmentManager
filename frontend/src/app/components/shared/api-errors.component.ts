import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-api-errors',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './api-errors.component.html',
})
export class ApiErrorsComponent {
  @Input() errors: string[] | null | undefined = [];
}
