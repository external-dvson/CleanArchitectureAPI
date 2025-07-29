using CleanArchitectureApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitectureApi.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Bio)
            .HasMaxLength(1000);
        
        builder.Property(p => p.UserId)
            .IsRequired();
        
        builder.HasIndex(p => p.UserId)
            .IsUnique();
    }
}
