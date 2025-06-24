using Domain.Lessons.Entities;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class LessonSectionProgressConfiguration : IEntityTypeConfiguration<LessonSectionProgress>
{
    public void Configure(EntityTypeBuilder<LessonSectionProgress> builder)
    {
        builder.HasKey(lsp => new { lsp.UserId, lsp.LessonSectionId });
        builder
            .HasOne<LessonSection>()
            .WithMany(ls => ls.UserProgresses)
            .HasForeignKey(lsp => lsp.LessonSectionId);
        builder
            .HasOne<User>()
            .WithMany(u => u.LessonSectionProgresses)
            .HasForeignKey(lsp => lsp.UserId);
    }
}
