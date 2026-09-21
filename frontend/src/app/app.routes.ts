import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { CourseFormComponent } from './components/courses/course-form.component';
import { PasswordComponent } from './components/auth/password.component';
import { WorkspaceComponent } from './components/workspace/workspace.component';
import { StudentFormComponent } from './components/students/student-form.component';
import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';
import { EnrollmentFormComponent } from './components/enrollments/enrollment-form.component';
import { DashboardComponent } from './components/workspace/dashboard/dashboard.component';
import { UsersComponent } from './components/workspace/users/users.component';
import { StudentsComponent } from './components/workspace/students/students.component';
import { EnrollmentsComponent } from './components/workspace/enrollments/enrollments.component';
import { CoursesComponent } from './components/workspace/courses/courses.component';
import { TeacherFormComponent } from './components/teachers/teacher-form.component';
import { TeacherEditComponent } from './components/teachers/teacher-edit.component';
import { TeachersComponent } from './components/workspace/teachers/teachers.component';
import { CourseDetailsComponent } from './components/courses/course-details.component';
import { CourseReportComponent } from './components/courses/course-report.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgot-password', component: PasswordComponent },
  { path: 'reset-password', component: PasswordComponent, data: { reset: true } },
  { path: 'dashboard', component: WorkspaceComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Secretary'] }, children: [{ path: '', component: DashboardComponent }] },
  { path: 'users', component: WorkspaceComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] }, children: [{ path: '', component: UsersComponent }] },
  { path: 'students', component: WorkspaceComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] }, children: [{ path: '', component: StudentsComponent }] },
  { path: 'students/new', component: StudentFormComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] } },
  { path: 'students/:id/edit', component: StudentFormComponent, canActivate: [authGuard] },
  { path: 'enrollments', component: WorkspaceComponent, canActivate: [authGuard], children: [{ path: '', component: EnrollmentsComponent }] },
  { path: 'enrollments/new', component: EnrollmentFormComponent, canActivate: [authGuard] },
  { path: 'courses', component: WorkspaceComponent, canActivate: [authGuard], children: [{ path: '', component: CoursesComponent }] },
  { path: 'courses/new', component: CourseFormComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Secretary'] } },
  { path: 'courses/:id/edit', component: CourseFormComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] } },
  { path: 'courses/:id/report', component: CourseReportComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Secretary'] } },
  { path: 'courses/:id', component: CourseDetailsComponent, canActivate: [authGuard] },
  { path: 'teachers', component: WorkspaceComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] }, children: [{ path: '', component: TeachersComponent }] },
  { path: 'teachers/new', component: TeacherFormComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] } },
  { path: 'teachers/:id/edit', component: TeacherEditComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] } }
];
