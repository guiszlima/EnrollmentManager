namespace EnrollmentManager.API.Services.Interfaces;

public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string body);
}