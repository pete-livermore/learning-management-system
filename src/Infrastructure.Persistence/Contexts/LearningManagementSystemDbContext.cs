using Domain.Lessons.Entities;
using Domain.Uploads.Entities;
using Domain.Users.Entities;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts
{
    public class LearningManagementSystemDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public LearningManagementSystemDbContext(
            DbContextOptions<LearningManagementSystemDbContext> options
        )
            : base(options) { }

        public required DbSet<User> DomainUsers { get; set; }
        public required DbSet<UploadFile> Files { get; set; }
        public required DbSet<Lesson> Lessons { get; set; }
        public required DbSet<LessonSectionContent> LessonSectionContents { get; set; }
        public required DbSet<LessonSection> LessonSections { get; set; }
        public required DbSet<LessonSectionProgress> LessonSectionProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(LearningManagementSystemDbContext).Assembly
            );
        }
    }
}
