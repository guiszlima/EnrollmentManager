namespace EnrollmentManager.API.Configurations;

public class EmailConfiguration
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    // TODO: Configurar no gmail o token de e-mail para autenticação, pois o gmail não permite mais autenticação com senha.
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
}