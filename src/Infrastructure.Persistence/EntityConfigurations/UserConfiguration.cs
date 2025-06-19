namespace Infrastructure.Persistence.EntityConfigurations;

using Domain.Users.Entities;
using Domain.Users.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasIndex(u => u.Email).IsUnique();
        builder
            .Property(u => u.Role)
            .HasConversion(v => v.ToString(), v => (UserRole)Enum.Parse(typeof(UserRole), v))
            .HasMaxLength(50);
    }
}
