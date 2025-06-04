using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class FileConfiguration : IEntityTypeConfiguration<UploadFile>
{
    public void Configure(EntityTypeBuilder<UploadFile> builder)
    {
        builder.HasOne(f => f.Owner).WithMany().HasForeignKey(f => f.OwnerId);
    }
}
