using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Student;
using EnrollmentManager.API.Models;
using EnrollmentManager.API.Services.Interfaces.Student;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Students;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentResponseDto>> GetAllAsync(StudentFilterDto filter)
    {
        var query = _context.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(student => student.User.UserName.Contains(filter.Name));
        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(student => student.User.Email.Contains(filter.Email));
        if (!string.IsNullOrWhiteSpace(filter.RegistrationNumber))
            query = query.Where(student => student.RegistrationNumber.Contains(filter.RegistrationNumber));
        if (!string.IsNullOrWhiteSpace(filter.Nationality))
            query = query.Where(student => student.Nationality != null && student.Nationality.Contains(filter.Nationality));
        if (!string.IsNullOrWhiteSpace(filter.Phone))
            query = query.Where(student => student.Phone.Contains(filter.Phone));

        return await query
            .AsNoTracking()
            .Select(student => new StudentResponseDto
            {
                UserId = student.UserId,
                UserName = student.User.UserName,
                Email = student.User.Email,
                Cpf = student.Cpf,
                PassportNumber = student.PassportNumber,
                Nationality = student.Nationality,
                BirthDate = student.BirthDate,
                Phone = student.Phone,
                Address = student.Address,
                RegistrationNumber = student.RegistrationNumber,
                FormatIds = student.AllowedFormats
                    .Select(format => format.FormatId)
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<StudentResponseDto?> GetByIdAsync(int userId)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(student => student.UserId == userId)
            .Select(student => new StudentResponseDto
            {
                UserId = student.UserId,
                UserName = student.User.UserName,
                Email = student.User.Email,
                Cpf = student.Cpf,
                PassportNumber = student.PassportNumber,
                Nationality = student.Nationality,
                BirthDate = student.BirthDate,
                Phone = student.Phone,
                Address = student.Address,
                RegistrationNumber = student.RegistrationNumber,
                FormatIds = student.AllowedFormats
                    .Select(format => format.FormatId)
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ApiResponseDto<StudentResponseDto>> CreateAsync(StudentCreateDto dto)
    {
        if (!ValidateNationalityAndDocuments(dto.Nationality, dto.Cpf, dto.PassportNumber))
            return ApiResponseDto<StudentResponseDto>.Error("Documento de identificação inválido para a nacionalidade informada.");

        var user = await _context.Users
      .Include(u => u.Role)
      .FirstOrDefaultAsync(u => u.Id == dto.UserId);

        if (user == null)
            return ApiResponseDto<StudentResponseDto>.Error("Usuário não encontrado no sistema.");

        
        if (user.Role?.Code != "STUDENT")
            return ApiResponseDto<StudentResponseDto>
                .Error("O usuário não possui o cargo de aluno.");

        bool hasStudentProfile = await _context.Students.AnyAsync(student => student.UserId == dto.UserId);
        if (hasStudentProfile)
            return ApiResponseDto<StudentResponseDto>.Error("Este usuário já possui um perfil de aluno cadastrado.");

        bool isBr = IsBrazilian(dto.Nationality);
        string? cleanCpf = isBr ? dto.Cpf : null;
        string? cleanPassport = isBr ? null : dto.PassportNumber;
        DateTime BirthDateUtc = DateTime.SpecifyKind(dto.BirthDate, DateTimeKind.Utc);
        string registrationNumber = GenerateRegistrationNumber();

        if (await HasDuplicateDocumentOrRegistrationAsync(cleanCpf, cleanPassport, registrationNumber))
            return ApiResponseDto<StudentResponseDto>.Error("Já existe um aluno cadastrado com esta Matrícula, CPF ou Passaporte.");

        if (!dto.FormatIds.Any())
            return ApiResponseDto<StudentResponseDto>.Error("É necessário informar pelo menos um formato de estudo.");

        if (!await FormatsExistAsync(dto.FormatIds))
            return ApiResponseDto<StudentResponseDto>.Error("Um ou mais formatos informados não existem.");
        
        var student = new EnrollmentManager.API.Models.Student
        {
            UserId = dto.UserId,
            Cpf = cleanCpf,
            PassportNumber = cleanPassport,
            Nationality = dto.Nationality,
            BirthDate = BirthDateUtc,
            Phone = dto.Phone,
            Address = dto.Address,
            RegistrationNumber = registrationNumber
        };

        AddFormats(student, dto.FormatIds);

        _context.Students.Add(student);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiResponseDto<StudentResponseDto>.Error("Erro inesperado ao salvar os dados do aluno.");
        }

        var resultDto = await GetByIdAsync(student.UserId);
        return new ApiResponseDto<StudentResponseDto> 
        { 
            Data = resultDto, 
            Message = "Aluno cadastrado com sucesso." 
        };
    }

    public async Task<ApiResponseDto<StudentResponseDto>> UpdateAsync(
    int userId,
    StudentUpdateDto dto)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .Include(s => s.AllowedFormats)
            .FirstOrDefaultAsync(current => current.UserId == userId);

        if (student is null)
            return ApiResponseDto<StudentResponseDto>.Error(
                "Aluno não encontrado.");

        if (!ValidateNationalityAndDocuments(
                dto.Nationality,
                dto.Cpf,
                dto.PassportNumber))
        {
            return ApiResponseDto<StudentResponseDto>.Error(
                "Documento de identificação inválido para a nacionalidade informada.");
        }

        if (!dto.FormatIds.Any())
            return ApiResponseDto<StudentResponseDto>.Error("É necessário informar pelo menos um formato de estudo.");

        bool isBr = IsBrazilian(dto.Nationality);

        string? cleanCpf = isBr ? dto.Cpf : null;
        string? cleanPassport = isBr ? null : dto.PassportNumber;

        student.Cpf = cleanCpf;
        student.PassportNumber = cleanPassport;
        student.Nationality = dto.Nationality;
        student.BirthDate = dto.BirthDate;
        student.Phone = dto.Phone;
        student.Address = dto.Address;

        if (!await FormatsExistAsync(dto.FormatIds))
            return ApiResponseDto<StudentResponseDto>.Error("Um ou mais formatos informados não existem.");

        student.AllowedFormats.Clear();
        AddFormats(student, dto.FormatIds);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiResponseDto<StudentResponseDto>.Error(
                "Erro inesperado ao atualizar os dados do aluno.");
        }

        var resultDto = await GetByIdAsync(student.UserId);

        return new ApiResponseDto<StudentResponseDto>
        {
            Data = resultDto,
            Message = "Aluno atualizado com sucesso."
        };
    }
    public async Task<ApiResponseDto<bool>> DeleteAsync(int userId)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(current => current.UserId == userId);

        if (student is null)
            return ApiResponseDto<bool>.Error("Aluno não encontrado.");

        bool hasEnrollments = await _context.Enrollments.AnyAsync(enrollment => enrollment.StudentId == userId);
        if (hasEnrollments)
            return ApiResponseDto<bool>.Error("Não é possível remover o aluno, pois existem matrículas ativas vinculadas a ele.");

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return new ApiResponseDto<bool> 
        { 
            Data = true, 
            Message = "Aluno removido com sucesso." 
        };
    }

    private async Task<bool> HasDuplicateDocumentOrRegistrationAsync(
        string? cpf,
        string? passportNumber,
        string registrationNumber,
        int? userId = null)
    {
        bool hasCpf = !string.IsNullOrWhiteSpace(cpf);
        bool hasPassport = !string.IsNullOrWhiteSpace(passportNumber);

        return await _context.Students.AnyAsync(student =>
            (!userId.HasValue || student.UserId != userId.Value) &&
            (student.RegistrationNumber == registrationNumber ||
             (hasCpf && student.Cpf == cpf) ||
             (hasPassport && student.PassportNumber == passportNumber)));
    }

    private static bool IsBrazilian(string? nationality) =>
        !string.IsNullOrWhiteSpace(nationality) &&
        nationality.Trim().Equals("Brasil", StringComparison.OrdinalIgnoreCase);


    private static string GenerateRegistrationNumber()
    {
        return $"{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }
    private static bool ValidateNationalityAndDocuments(string? nationality, string? cpf, string? passportNumber)
    {
        if (string.IsNullOrWhiteSpace(nationality))
            return false;

        if (IsBrazilian(nationality))
            return !string.IsNullOrWhiteSpace(cpf);

        return !string.IsNullOrWhiteSpace(passportNumber);
    }

    private async Task<bool> FormatsExistAsync(IEnumerable<int> formatIds)
    {
        // O HashSet já remove os duplicados na criação e é mais rápido para buscas
        var ids = formatIds.ToHashSet();

        if (ids.Count == 0)
            return false;

        var existingCount = await _context.StudyFormats
            .CountAsync(format => ids.Contains(format.Id));

        return existingCount == ids.Count;
    }

    private static void AddFormats(Student student, IEnumerable<int> formatIds)
    {
        foreach (var formatId in formatIds.Distinct())
        {
            student.AllowedFormats.Add(new StudentStudyFormat
            {
                Student = student,
                StudentId = student.UserId,
                FormatId = formatId
            });
        }
    }
}
