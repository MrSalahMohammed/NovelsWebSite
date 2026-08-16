using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novels.Domain.Entities;

namespace Novels.Infrastructure.Data.DataConfigurations
{
    public class NovelConfig : IEntityTypeConfiguration<Novel>
    {
        public void Configure(EntityTypeBuilder<Novel> builder)
        {
            builder.ToTable("Novels");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Name)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(n => n.Slug)
                .IsRequired()
                .HasMaxLength(350);

            builder.HasIndex(n => n.Slug)
                .IsUnique();

            builder.Property(n => n.Description)
                .HasMaxLength(4000);

            builder.Property(n => n.CoverImageUrl)
                .HasMaxLength(500);

            builder.Property(n => n.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(n => n.AverageRating)
                .HasColumnType("decimal(3,2)");

            builder.Property(n => n.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasMany(n => n.Categories)
                .WithMany(c => c.Novels)
                .UsingEntity(j => j.ToTable("NovelCategories"));

            builder.HasMany(n => n.Tags)
                .WithMany(t => t.Novels)
                .UsingEntity(j => j.ToTable("NovelTags"));

            builder.HasMany(n => n.Chapters)
                .WithOne(c => c.Novel)
                .HasForeignKey(c => c.NovelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(n => n.Reviews)
                .WithOne(r => r.Novel)
                .HasForeignKey(r => r.NovelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
