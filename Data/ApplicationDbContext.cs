using System.Text.Json;
using CQRS_Microservice.Models;
using Microsoft.EntityFrameworkCore;

namespace CQRS_Microservice.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        optionsBuilder.UseNpgsql("Your PostgreSQL connection string here");
    }
}

        public DbSet<Product> Products { get; set; }
         public DbSet<User> Users { get; set; }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Permissions)
                .HasColumnType("jsonb") // Specify JSONB column type
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null), // Serialize list to JSON
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)); // Deserialize JSON to list
        });
    }
}
}