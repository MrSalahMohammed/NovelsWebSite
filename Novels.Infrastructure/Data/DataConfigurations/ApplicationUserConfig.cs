using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novels.Domain.Entities;

namespace Novels.Infrastructure.Data.DataConfigurations
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.RecoveryEmail)
                .HasMaxLength(256);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); 

            builder.HasMany(u => u.FavoriteNovels)
                .WithMany(n => n.FavoritedBy)
                .UsingEntity(j => j.ToTable("NovelFavorites"));

            builder.HasMany(u => u.Reviews)
                .WithOne(r => r.Reader)
                .HasForeignKey(r => r.ReaderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.ReadingHistory)
                .WithOne(rp => rp.Reader)
                .HasForeignKey(rp => rp.ReaderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
