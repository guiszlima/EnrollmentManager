import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';

interface NavItem {
  label: string;
  path: string;
  icon: string;
  roles: string[];
}

@Component({
  selector: 'app-workspace',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterOutlet],
  templateUrl: './workspace.component.html'
})
export class WorkspaceComponent {
  readonly navItems: NavItem[] = [
    { label: 'Dashboard', path: '/dashboard', icon: '01', roles: ['Admin', 'Secretary'] },
    { label: 'Cursos', path: '/courses', icon: '02', roles: ['Admin', 'Secretary', 'Student'] },
    { label: 'Alunos', path: '/students', icon: '03', roles: ['Admin'] },
    { label: 'Matrículas', path: '/enrollments', icon: '04', roles: ['Admin', 'Secretary', 'Student'] },
    { label: 'Usuários', path: '/users', icon: '05', roles: ['Admin'] }
    ,{ label: 'Professores', path: '/teachers', icon: '06', roles: ['Admin'] }
  ];

  constructor(private auth: AuthService, public router: Router) {}

  get isStaff(): boolean {
    return this.auth.hasAnyRole(['Admin', 'Secretary']);
  }

  get pageTitle(): string {
    const path = this.router.url.split('?')[0];
    return ({
      '/dashboard': 'Dashboard',
      '/users': 'Usuários',
      '/students': 'Alunos',
      '/enrollments': 'Matrículas'
      ,'/teachers': 'Professores'
    } as Record<string, string>)[path] ?? 'Workspace';
  }

  get pageDescription(): string {
    const path = this.router.url.split('?')[0];
    return ({
      '/dashboard': 'Acompanhe os principais registros da operação acadêmica.',
      '/users': 'Gerencie acesso, status e responsabilidades da equipe.',
      '/students': 'Centralize informações de contato e identificação dos alunos.',
      '/enrollments': 'Acompanhe o percurso de cada aluno na instituição.'
      ,'/teachers': 'Gerencie os professores e os formatos que eles lecionam.'
    } as Record<string, string>)[path] ?? '';
  }

  canSee(item: NavItem): boolean {
    return this.auth.hasAnyRole(item.roles);
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigate(['/login']);
  }
}
