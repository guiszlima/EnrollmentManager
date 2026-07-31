using Microsoft.EntityFrameworkCore;
using EnrollmentManager.API.Models;
namespace EnrollmentManager.API.Data;

public class ApplicationDbContext : DbContext
{
  public  ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
} 