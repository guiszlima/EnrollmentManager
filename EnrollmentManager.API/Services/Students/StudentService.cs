using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOS.Student;
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

    public async Task<List<StudentResponseDTO>> GetAllAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .Select(student => new StudentResponseDTO
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

    public async Task<StudentResponseDTO?> GetByIdAsync(int userId)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(student => student.UserId == userId)
            .Select(student => new StudentResponseDTO
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

    public async Task<StudentResponseDTO?> CreateAsync(StudentCreateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Cpf) && string.IsNullOrWhiteSpace(dto.PassportNumber))
            return null;

        bool userExists = await _context.Users
            .AnyAsync(user => user.Id == dto.UserId);

        if (!userExists || await _context.Students.AnyAsync(student => student.UserId == dto.UserId))
            return null;

        if (await HasDuplicateDocumentOrRegistrationAsync(dto.Cpf, dto.PassportNumber, dto.RegistrationNumber))
            return null;

        var student = new Student
        {
            UserId = dto.UserId,
            Cpf = dto.Cpf,
            PassportNumber = dto.PassportNumber,
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
            return null;
        }

        return await GetByIdAsync(student.UserId);
    }

    public async Task<StudentResponseDTO?> UpdateAsync(int userId, StudentUpdateDTO dto)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(current => current.UserId == userId);

        if (student is null)
            return null;

        if (string.IsNullOrWhiteSpace(dto.Cpf) && string.IsNullOrWhiteSpace(dto.PassportNumber))
            return null;

        if (await HasDuplicateDocumentOrRegistrationAsync(
                dto.Cpf,
                dto.PassportNumber,
                dto.RegistrationNumber,
                userId))
            return null;

        student.Cpf = dto.Cpf;
        student.PassportNumber = dto.PassportNumber;
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
            return null;
        }

        return await GetByIdAsync(student.UserId);
    }

    public async Task<bool> DeleteAsync(int userId)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(current => current.UserId == userId);

        if (student is null)
            return false;

        if (await _context.Enrollments.AnyAsync(enrollment => enrollment.StudentId == userId))
            return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<bool> HasDuplicateDocumentOrRegistrationAsync(
        string? cpf,
        string? passportNumber,
        string registrationNumber,
        int? userId = null)
    {
        return await _context.Students.AnyAsync(student =>
            (!userId.HasValue || student.UserId != userId.Value) &&
            (student.RegistrationNumber == registrationNumber ||
             (cpf != null && student.Cpf == cpf) ||
             (passportNumber != null && student.PassportNumber == passportNumber)));
    }

}
