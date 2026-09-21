import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  forkJoin,
  Observable
} from 'rxjs';

import { AdminService } from '../../../services/admin.service';
import { CatalogService } from '../../../services/catalog.service';

import {
  AdminUserDto,
  RoleResponseDto
} from '../../../models/admin';

import {
  ApiResponse
} from '../../../models/api-response';

import {
  extractApiErrors
} from '../../../utills/api-errors-utills';

import {
  ApiErrorsComponent
} from '../../../components/shared/api-errors.component';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ApiErrorsComponent
  ],
  templateUrl: './users.component.html'
})
export class UsersComponent implements OnInit {

  users: AdminUserDto[] = [];

  filteredUsers: AdminUserDto[] = [];

  roles: RoleResponseDto[] = [];

  approvalRoleId: Record<number, number> = {};

  activeUsers = true;

  query = '';

  loading = false;

  errorMessage = '';

  successMessage = '';

  errors: string[] = [];

  constructor(
    private admin: AdminService,
    private catalog: CatalogService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    this.errors = [];

    forkJoin({
      roles: this.catalog.roles(),
      users: this.admin.users(this.activeUsers)
    }).subscribe({
      next: ({ roles, users }) => {
        this.roles = roles.data ?? [];
        this.users = users.data ?? [];

        this.initializeRoles();
        this.filterUsers();

        this.loading = false;

        this.cdr.detectChanges();
      },

      error: error => {
        this.fail(error);
      }
    });
  }

  setUserFilter(active: boolean): void {
    if (this.activeUsers === active) {
      return;
    }

    this.activeUsers = active;
    this.query = '';

    this.load();
  }

  filterUsers(): void {
    const query = this.query
      .trim()
      .toLowerCase();

    this.filteredUsers = this.users.filter(user => {
      const username = user.username ?? '';
      const email = user.email ?? '';
      const role = user.role ?? '';

      const searchableText = [
        username,
        email,
        role
      ]
        .join(' ')
        .toLowerCase();

      return searchableText.includes(query);
    });
  }

  approve(user: AdminUserDto): void {
    const roleId = this.approvalRoleId[user.id];

    if (!roleId) {
      this.errors = ['Selecione um papel antes de aprovar o usuário.'];
      this.errorMessage = this.errors[0];

      return;
    }

    this.runAction(
      this.admin.approve(
        user.id,
        { roleId }
      ),
      'Usuário aprovado com sucesso.'
    );
  }

  changeRole(user: AdminUserDto): void {
    const roleId =
      this.approvalRoleId[user.id];

    if (!roleId) {
      this.errors = ['Selecione um papel antes de alterar o usuário.'];
      this.errorMessage = this.errors[0];

      return;
    }

    this.runAction(
      this.admin.changeRole(
        user.id,
        { roleId }
      ),
      'Papel do usuário atualizado com sucesso.'
    );
  }

  resetPassword(user: AdminUserDto): void {
    this.runAction(
      this.admin.resetPassword(user.id),
      'Solicitação de redefinição enviada com sucesso.',
      false
    );
  }

  removeUser(user: AdminUserDto): void {
    const confirmed = confirm(
      `Excluir o usuário ${user.username}?`
    );

    if (!confirmed) {
      return;
    }

    this.runAction(
      this.admin.remove(user.id),
      'Usuário removido com sucesso.'
    );
  }

  private initializeRoles(): void {
    this.approvalRoleId = {};

    for (const user of this.users) {
      if (!user.role) {
        this.approvalRoleId[user.id] =
          this.roles[0]?.id ?? 0;

        continue;
      }

      const currentRole =
        user.role.toLowerCase();

      const matchingRole =
        this.roles.find(role =>
          role.name.toLowerCase() === currentRole
        );

      this.approvalRoleId[user.id] =
        matchingRole?.id ??
        this.roles[0]?.id ??
        0;
    }
  }

  private runAction<T>(
    request: Observable<ApiResponse<T>>,
    successMessage: string,
    reload = true
  ): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.errors = [];

    request.subscribe({
      next: response => {
        this.successMessage =
          response.message ?? successMessage;

        if (reload) {
          this.load();
        }

        this.cdr.detectChanges();
      },

      error: error => {
        this.fail(error);
      }
    });
  }

  private fail(error: unknown): void {
    this.errors =
      extractApiErrors(error);

    this.errorMessage =
      this.errors[0] ??
      'Não foi possível concluir a operação.';

    this.successMessage = '';
    this.loading = false;

    this.cdr.detectChanges();
  }
}
