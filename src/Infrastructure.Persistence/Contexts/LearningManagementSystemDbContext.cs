using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts
{
    public class LearningManagementSystemDbContext : DbContext
    {
        public LearningManagementSystemDbContext(
            DbContextOptions<LearningManagementSystemDbContext> options
        )
            : base(options) { }

        public required DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(LearningManagementSystemDbContext).Assembly
            );
        }
    }
}
