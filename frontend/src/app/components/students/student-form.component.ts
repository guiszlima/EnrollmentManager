import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import {
  StudentCreateDto,
  StudentUpdateDto,
  UserStudentCreateDto,
} from '../../models/student';
import { StudyFormatDto } from '../../models/catalog';
import { UserResponseDto } from '../../models/user';

import { StudentService } from '../../services/student.service';
import { UserService } from '../../services/user.service';
import { CatalogService } from '../../services/catalog.service';

import { extractApiErrors } from '../../utills/api-errors-utills';
import { ApiErrorsComponent } from '../shared/api-errors.component';

import { NATIONALITIES } from '../../constants/nacionalities';

@Component({
  selector: 'app-student-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './student-form.component.html',
})
export class StudentFormComponent implements OnInit {
  public edit = false;

  public id: number | null = null;

  public loading = false;

  public loadingUsers = false;

  public userExists = true;

  public errors: string[] = [];

  public currentUserLabel = '';

  public readonly nationalities = NATIONALITIES;

  public model = {
    userId: 0,
    userName: '',
    email: '',
    password: '',
    cpf: '',
    passportNumber: '',
    nationality: '',
    birthDate: '',
    phone: '',
    address: '',
    formatIds: [] as number[],
  };

  public availableFormats: StudyFormatDto[] = [];

  public availableUsers: UserResponseDto[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: StudentService,
    private userService: UserService,
    private catalog: CatalogService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.edit = Boolean(id);
    this.id = id || null;

    this.loadAvailableUsers();

    this.catalog.studyFormats().subscribe({
      next: (response: { data?: StudyFormatDto[] | null }) => {
        this.availableFormats = response.data ?? [];
        this.cdr.markForCheck();
      },
      error: (error: unknown) => {
        this.errors = extractApiErrors(error);
        this.cdr.markForCheck();
      },
    });
    if (id) {
      this.service.getById(id).subscribe({
        next: (response) => {
          const student = response.data;

          if (student) {
            this.model = {
              userId: student.userId,
              userName: '',
              email: '',
              password: '',
              cpf: student.cpf ?? '',
              passportNumber: student.passportNumber ?? '',
              nationality: student.nationality ?? '',
              birthDate: student.birthDate
                ? student.birthDate.substring(0, 10)
                : '',
              phone: student.phone ?? '',
              address: student.address ?? '',
              formatIds: student.formatIds ?? [],
            };

            this.currentUserLabel = `${
              student.userName || (student as any).username
            } · ${student.email}`;
          }
          this.cdr.markForCheck();
        },
        error: (error) => {
          this.errors = extractApiErrors(error);
          this.cdr.detectChanges();
        },
      });
    }
  }

  public onNationalityChange(): void {
    if (!this.model.nationality) {
      this.model.cpf = '';
      this.model.passportNumber = '';
      return;
    }

    if (this.model.nationality === 'Brasil') {
      this.model.passportNumber = '';
    } else {
      this.model.cpf = '';
    }
  }

  public save(): void {
    this.loading = true;
    this.errors = [];
    this.model.formatIds = this.model.formatIds.map(Number).filter(id => id > 0);
    const payloadAddress = this.model.address ? this.model.address.trim() : '';
    if (this.model.formatIds.length === 0) {
      this.errors = ['É necessário informar pelo menos um formato de estudo.'];
      this.loading = false;
      return;
    }

    const payloadCpf = this.model.cpf ? this.model.cpf : null;

    const payloadPassport = this.model.passportNumber
      ? this.model.passportNumber
      : null;

    let call;

    if (this.edit && this.id) {
      const updateDto: StudentUpdateDto = {
        cpf: payloadCpf,
        passportNumber: payloadPassport,
        nationality: this.model.nationality,
        birthDate: this.model.birthDate,
        phone: this.model.phone,
        address: payloadAddress,
        formatIds: this.model.formatIds,
      };

      call = this.service.update(this.id, updateDto);
    } else {
      if (this.userExists) {
        const createDto: StudentCreateDto = {
          userId: this.model.userId,
          cpf: payloadCpf,
          passportNumber: payloadPassport,
          nationality: this.model.nationality,
          birthDate: this.model.birthDate,
          phone: this.model.phone,
          address: this.model.address,
          formatIds: this.model.formatIds,
        };

        call = this.service.create(createDto);
      } else {
        const createDto: UserStudentCreateDto = {
          userName: this.model.userName,
          email: this.model.email,
          password: this.model.password,
          cpf: payloadCpf,
          passportNumber: payloadPassport,
          nationality: this.model.nationality,
          birthDate: this.model.birthDate,
          phone: this.model.phone,
          address: this.model.address,
          formatIds: this.model.formatIds,
        };

        call = this.service.createWithUser(createDto);
      }
    }

    call.subscribe({
      next: () => this.router.navigate(['/students']),
      error: (error) => {
        this.errors = extractApiErrors(error);
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  private loadAvailableUsers(): void {
    if (this.edit) {
      return;
    }

    this.loadingUsers = true;

    this.userService.getAvailableForStudent().subscribe({
      next: (response) => {
        this.availableUsers = response.data ?? [];
        this.loadingUsers = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        this.errors = extractApiErrors(error);
        this.loadingUsers = false;
        this.cdr.detectChanges();
      },
    });
  }
}
