using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novels.Domain.Entities;

namespace Novels.Infrastructure.Data.DataConfigurations
{
    public class ReadingProgressConfig : IEntityTypeConfiguration<ReadingProgress>
    {
        public void Configure(EntityTypeBuilder<ReadingProgress> builder)
        {
            builder.ToTable("ReadingProgresses");

            builder.HasKey(rp => rp.Id);

            builder.Property(rp => rp.LastReadAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(rp => new { rp.ReaderId, rp.NovelId })
                .IsUnique();

            builder.HasOne(rp => rp.Novel)
                .WithMany() 
                .HasForeignKey(rp => rp.NovelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.LastChapter)
                .WithMany()
                .HasForeignKey(rp => rp.LastChapterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
