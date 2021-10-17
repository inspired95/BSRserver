using Microsoft.EntityFrameworkCore;
using OperationsService.Data.Models;

namespace OperationsService.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }

        public DbSet<Operation> Operations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Operation>()
                .ToTable("Operations");

            modelBuilder.Entity<Operation>()
                .Property(c => c.Bank)
                .HasConversion<string>();

            modelBuilder.Entity<Operation>()
                .Property(c => c.Type)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
