# Mapa de processos

## 1. Acesso e identidade

Controller: `AuthController` (`/api/auth`)

- Entrar: `POST /login`, armazenar o JWT e abrir o workspace.
- Criar acesso: `POST /register`, confirmar o cadastro e voltar ao login.
- Recuperar acesso: `POST /forgot-password` e `POST /reset-password`.

## 2. Catálogo acadêmico

Controllers: `CourseController`, `CourseTypeController`, `EducationLevelController`, `CourseStatusController` e `CourseStudyFormatController`.

- Consultar cursos: `GET /api/courses`.
- Criar curso: `POST /api/courses` (Staff).
- Editar curso: `PUT /api/courses/{id}` (Admin).
- Excluir curso: `DELETE /api/courses/{id}` (Admin).
- Alimentar seleções do formulário com tipos, níveis, status e formatos.

A interface atual cobre a consulta, busca, filtro, criação, edição e exclusão de cursos. O formulário ainda recebe os IDs dos catálogos diretamente; a próxima evolução deve consumir os endpoints de catálogo para trocar esses campos por seleções nomeadas.

## 3. Jornada de matrícula

Controllers: `EnrollmentsController` e `EnrollmentStatusController`.

- Solicitar matrícula: `POST /api/enrollments` com aluno, curso e formato.
- Consultar uma matrícula: `GET /api/enrollments/{id}`.
- Acompanhar matrículas do aluno: `GET /api/enrollments/student/{studentId}`.
- Atualizar andamento: `PATCH /api/enrollments/{id}/status` (Staff).
- Usar os status de matrícula em filtros e indicadores.

## 4. Pessoas e administração

Controllers: `StudentController`, `AdminController` e `RoleController`.

- Consultar e editar o próprio perfil: `GET/PUT /api/students/{userId}`.
- Gestão de alunos: `GET/POST/DELETE /api/students` (Admin).
- Aprovar usuários: `PATCH /api/admin/users/{id}/approve`.
- Alterar perfil de acesso: `PATCH /api/admin/users/{id}/role`.
- Consultar roles: `GET /api/roles`.
- Desativar usuário ou iniciar reset administrativo.

Esses processos estão mapeados para as próximas telas do workspace. A navegação visual já reserva os contextos de cursos, matrículas e pessoas sem simular operações que ainda não possuem serviços no frontend.
