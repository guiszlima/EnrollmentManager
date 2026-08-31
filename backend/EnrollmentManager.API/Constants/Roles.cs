namespace EnrollmentManager.API.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Secretary = "Secretary";
    public const string Student = "Student";

    // Agrupamentos úteis para autorização
    public const string Staff = $"{Admin},{Secretary}";
    public const string All = $"{Admin},{Secretary},{Student}";
}