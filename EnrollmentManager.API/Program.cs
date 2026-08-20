
using EnrollmentManager.API.Data;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddApplicationServices(); // Adiciona os serviços da aplicação (AuthService, TokenService, etc.)

        var app = builder.Build();

        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            
            app.UseSwaggerUI(options =>
            {
                
                options.SwaggerEndpoint("/openapi/v1.json", "Enrollment Manager API v1");
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
