
using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.Extensions;
using EnrollmentManager.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace EnrollmentManager.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Default database connection is not configured.");
        
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        builder.Services.AddControllers();
        builder.Services.AddApplicationSettings(builder.Configuration);
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddAuthorization();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi(options =>
        {
            options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
        });
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

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
