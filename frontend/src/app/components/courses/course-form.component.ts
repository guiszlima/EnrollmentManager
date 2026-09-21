import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CourseService } from '../../services/course.service';
import { CatalogService } from '../../services/catalog.service';
import { CourseInputDto } from '../../models/course';
import {
  CourseTypeResponseDto,
  CourseStatusResponseDto,
  EducationLevelResponseDto,
} from '../../models/catalog';
import { ApiErrorsComponent } from '../shared/api-errors.component';
import { extractApiErrors } from '../../utills/api-errors-utills';
import { TeacherService } from '../../services/teacher.service';
import { TeacherResponseDto } from '../../models/teacher';
import { StudyFormatDto } from '../../models/catalog';

@Component({
  selector: 'app-course-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ApiErrorsComponent],
  templateUrl: './course-form.component.html',
})
export class CourseFormComponent implements OnInit {
  model: CourseInputDto = { name: '', courseTypeId: 0, educationLevelId: 0, courseStatusId: 0, formatIds: [], teacherIds: [], totalSlots: 1 };
  isEdit = false;
  loading = false;
  errors: string[] = [];
  courseId: number | null = null;
  courseTypes: CourseTypeResponseDto[] = [];
  educationLevels: EducationLevelResponseDto[] = [];
  courseStatuses: CourseStatusResponseDto[] = [];
  formats: StudyFormatDto[] = [];
  teachers: TeacherResponseDto[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private courseService: CourseService,
    private catalog: CatalogService,
    private teacherService: TeacherService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.catalog.courseTypes().subscribe((response) => (this.courseTypes = response.data ?? []));
    this.catalog
      .educationLevels()
      .subscribe((response) => (this.educationLevels = response.data ?? []));
    this.catalog
      .courseStatuses()
      .subscribe((response) => (this.courseStatuses = response.data ?? []));
    this.catalog.studyFormats().subscribe({
      next: response => {
        this.formats = response.data ?? [];
        this.cdr.markForCheck();
      },
      error: error => this.setErrors(error),
    });
    this.teacherService.getAll().subscribe({
      next: response => {
        this.teachers = response.data ?? [];
        this.cdr.markForCheck();
      },
      error: error => this.setErrors(error),
    });
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit = true;
      this.courseId = id;
      this.loadCourse(id);
    }
  }

  loadCourse(id: number): void {
    this.courseService.getById(id).subscribe({
      next: (response) => {
        const course = response.data;
        if (course)
          this.model = {
            name: course.name,
            courseTypeId: course.courseTypeId,
            educationLevelId: course.educationLevelId,
            courseStatusId: course.statusId,
            formatIds: course.formatIds ?? [],
            teacherIds: course.teacherIds ?? [],
            totalSlots: course.totalSlots,
          };
      },
      error: (error) => this.setErrors(error),
    });
  }

  save(): void {
    this.loading = true;
    this.errors = [];
    this.model.formatIds = this.model.formatIds.map(Number).filter(id => id > 0);
    if (this.model.formatIds.length === 0) {
      this.errors = ['É necessário informar pelo menos um formato de estudo.'];
      this.loading = false;
      return;
    }
    const operation =
      this.isEdit && this.courseId !== null
        ? this.courseService.update(this.courseId, this.model)
        : this.courseService.create(this.model);
    operation.subscribe({
      next: () => this.router.navigate(['/courses']),
      error: (error) => {
        this.setErrors(error);
        this.loading = false;
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/courses']);
  }

  private setErrors(error: unknown): void {
    this.errors = extractApiErrors(error);
    this.cdr.detectChanges();
  }
}
