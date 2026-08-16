using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novels.Domain.Entities;

namespace Novels.Infrastructure.Data.DataConfigurations
{
    public class ChapterConfig : IEntityTypeConfiguration<Chapter>
    {
        public void Configure(EntityTypeBuilder<Chapter> builder)
        {
            builder.ToTable("Chapters");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(c => c.IsPublished)
                .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(c => new { c.NovelId, c.ChapterNumber })
                .IsUnique();
        }
    }
}
