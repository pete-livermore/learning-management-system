using Domain.Lessons.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class MarkdownContentConfiguration : IEntityTypeConfiguration<MarkdownContent>
{
    public void Configure(EntityTypeBuilder<MarkdownContent> builder)
    {
        builder.OwnsOne(m => m.Markdown);
    }
}
