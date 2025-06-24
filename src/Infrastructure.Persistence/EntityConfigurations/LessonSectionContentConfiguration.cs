using Domain.Lessons.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class LessonContentConfiguration : IEntityTypeConfiguration<LessonSectionContent>
{
    public void Configure(EntityTypeBuilder<LessonSectionContent> builder)
    {
        builder
            .HasDiscriminator<string>("content_type")
            .HasValue<MarkdownContent>("markdown")
            .HasValue<MediaContent>("media");
    }
}
