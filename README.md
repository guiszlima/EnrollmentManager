# EnrollmentManager

Sistema de gestão acadêmica para administrar usuários, alunos, professores, cursos e matrículas.

O projeto é dividido em uma API .NET e uma aplicação Angular. As regras de negócio ficam nos Services; Controllers recebem as requisições e retornam as respostas padronizadas, enquanto os DTOs separam os contratos de criação, atualização, consulta e relatório.

## Principais recursos

- Autenticação com JWT e controle de acesso por papel (`ADMIN`, `SECRETARY`, `TEACHER` e `STUDENT`).
- Cadastro e edição de alunos e professores, incluindo os formatos de estudo permitidos.
- Cadastro, edição e filtros de cursos.
- Matrículas vinculadas exclusivamente por `Enrollment` (`Student -> Enrollment -> Course`).
- Controle de vagas disponíveis e liberação de vaga para matrículas trancadas, canceladas ou concluídas.
- Validação de compatibilidade entre formatos de Student, Course e Teacher.
- Dashboard de Students, Teachers, Courses e Enrollments com filtros.
- Relatório detalhado de um curso e Relatório Geral de Cursos para impressão/salvamento em PDF.
- Respostas de erro padronizadas através do array `Errors`.

## Tecnologias

- Backend: .NET, ASP.NET Core, Entity Framework Core, PostgreSQL e JWT.
- Frontend: Angular 22, TypeScript, RxJS e Tailwind CSS.
- Testes: xUnit, Moq e `Microsoft.AspNetCore.Mvc.Testing` conforme os projetos de teste.

## Estrutura

```text
backend/
  EnrollmentManager.API/      API, Controllers, Services, DTOs e persistência
  EnrollmentManager.Tests/    testes automatizados dos Services
frontend/                     aplicação Angular
```

## Pré-requisitos

- .NET SDK compatível com `backend/EnrollmentManager.API/EnrollmentManager.API.csproj`.
- Node.js e npm.
- PostgreSQL acessível pela aplicação.

## Configuração do backend

Copie `backend/EnrollmentManager.API/appsettings.json.example.json` para `appsettings.json` e ajuste:

- `ConnectionStrings:DefaultConnection` para o banco PostgreSQL;
- `Jwt:Key`, `Jwt:Issuer` e `Jwt:Audience`;
- configurações de e-mail, caso o fluxo de e-mail seja utilizado;
- `Admin:Email` e `Admin:Password` para o usuário administrador criado pelo seeder.

O arquivo `appsettings.json` local não deve ser versionado.

## Executando localmente

Em um terminal, inicie a API:

```bash
cd backend
dotnet restore EnrollmentManager.sln
dotnet run --project EnrollmentManager.API
```

Em outro terminal, inicie o frontend:

```bash
cd frontend
npm install
npm start
```

A interface fica disponível em `http://localhost:4200`. A URL da API usada pelo frontend está em `frontend/src/environments/`.

## Testes e build

Testes do backend:

```bash
cd backend
dotnet test EnrollmentManager.sln
```

Build do frontend:

```bash
cd frontend
npm run build
```

## Endpoints principais

- `POST /api/auth/login` — autenticação.
- `GET /api/students` — dashboard filtrável de alunos.
- `GET /api/teachers` — dashboard filtrável de professores.
- `GET /api/courses` — dashboard filtrável de cursos.
- `GET /api/enrollments` — matrículas para usuários administrativos; retorna todos os status.
- `GET /api/courses/{courseId}/report` — relatório detalhado de um curso.

O acesso aos dados de matrícula é limitado por papel: usuários administrativos podem consultar todas as matrículas; `STUDENT` e `TEACHER` consultam somente os registros relacionados a si mesmos, conforme as regras do domínio.

## Regras importantes

- O `RegistrationNumber` do Student é gerado exclusivamente pelo `StudentService`.
- Um usuário administrador não pode ser excluído.
- Uma matrícula só pode ser aprovada quando o usuário do aluno está ativo e possui o papel `STUDENT`.
- Matrículas trancadas não ocupam vagas nem entram na contagem de matrículas ativas do curso.
- O dashboard de Student expõe apenas Courses e Enrollments; o dashboard geral é reservado aos papéis administrativos.

