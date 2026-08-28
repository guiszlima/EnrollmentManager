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

    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        return await _context.Students
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
                RegistrationNumber = student.RegistrationNumber
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
                RegistrationNumber = student.RegistrationNumber
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ApiResponseDto<StudentResponseDto>> CreateAsync(StudentCreateDto dto)
    {
        if (!ValidateNationalityAndDocuments(dto.Nationality, dto.Cpf, dto.PassportNumber))
            return ApiResponseDto<StudentResponseDto>.Error("Documento de identificação inválido para a nacionalidade informada.");

        bool userExists = await _context.Users.AnyAsync(user => user.Id == dto.UserId);
        if (!userExists)
            return ApiResponseDto<StudentResponseDto>.Error("Usuário não encontrado no sistema.");

        bool hasStudentProfile = await _context.Students.AnyAsync(student => student.UserId == dto.UserId);
        if (hasStudentProfile)
            return ApiResponseDto<StudentResponseDto>.Error("Este usuário já possui um perfil de aluno cadastrado.");

        bool isBr = IsBrazilian(dto.Nationality);
        string? cleanCpf = isBr ? dto.Cpf : null;
        string? cleanPassport = isBr ? null : dto.PassportNumber;

        if (await HasDuplicateDocumentOrRegistrationAsync(cleanCpf, cleanPassport, dto.RegistrationNumber))
            return ApiResponseDto<StudentResponseDto>.Error("Já existe um aluno cadastrado com esta Matrícula, CPF ou Passaporte.");

        var student = new EnrollmentManager.API.Models.Student
        {
            UserId = dto.UserId,
            Cpf = cleanCpf,
            PassportNumber = cleanPassport,
            Nationality = dto.Nationality,
            BirthDate = dto.BirthDate,
            Phone = dto.Phone,
            Address = dto.Address,
            RegistrationNumber = dto.RegistrationNumber
        };

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

    public async Task<ApiResponseDto<StudentResponseDto>> UpdateAsync(int userId, StudentUpdateDto dto)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(current => current.UserId == userId);

        if (student is null)
            return ApiResponseDto<StudentResponseDto>.Error("Aluno não encontrado.");

        if (!ValidateNationalityAndDocuments(dto.Nationality, dto.Cpf, dto.PassportNumber))
            return ApiResponseDto<StudentResponseDto>.Error("Documento de identificação inválido para a nacionalidade informada.");

        bool isBr = IsBrazilian(dto.Nationality);
        string? cleanCpf = isBr ? dto.Cpf : null;
        string? cleanPassport = isBr ? null : dto.PassportNumber;

        if (await HasDuplicateDocumentOrRegistrationAsync(cleanCpf, cleanPassport, dto.RegistrationNumber, userId))
            return ApiResponseDto<StudentResponseDto>.Error("Já existe outro aluno cadastrado com esta Matrícula, CPF ou Passaporte.");

        student.Cpf = cleanCpf;
        student.PassportNumber = cleanPassport;
        student.Nationality = dto.Nationality;
        student.BirthDate = dto.BirthDate;
        student.Phone = dto.Phone;
        student.Address = dto.Address;
        student.RegistrationNumber = dto.RegistrationNumber;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiResponseDto<StudentResponseDto>.Error("Erro inesperado ao atualizar os dados do aluno.");
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

    private static bool ValidateNationalityAndDocuments(string? nationality, string? cpf, string? passportNumber)
    {
        if (string.IsNullOrWhiteSpace(nationality))
            return false;

        if (IsBrazilian(nationality))
            return !string.IsNullOrWhiteSpace(cpf);

        return !string.IsNullOrWhiteSpace(passportNumber);
    }
}