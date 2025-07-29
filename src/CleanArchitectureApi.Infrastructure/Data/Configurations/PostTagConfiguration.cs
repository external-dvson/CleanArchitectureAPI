using CleanArchitectureApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitectureApi.Infrastructure.Data.Configurations;

public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
{
    public void Configure(EntityTypeBuilder<PostTag> builder)
    {
        // Composite primary key
        builder.HasKey(pt => new { pt.PostId, pt.TagId });
        
        builder.Property(pt => pt.PostId)
            .IsRequired();
        
        builder.Property(pt => pt.TagId)
            .IsRequired();
        
        // Relationships are configured in Post and Tag configurations
    }
}
