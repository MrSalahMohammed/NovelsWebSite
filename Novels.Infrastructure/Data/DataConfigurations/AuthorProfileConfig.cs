using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novels.Domain.Entities;

namespace Novels.Infrastructure.Data.DataConfigurations
{
    public class AuthorProfileConfig : IEntityTypeConfiguration<AuthorProfile>
    {
        public void Configure(EntityTypeBuilder<AuthorProfile> builder)
        {
            builder.ToTable("AuthorProfiles");

            builder.HasKey(ap => ap.Id);

            builder.Property(ap => ap.Bio)
                .HasMaxLength(2000);

            builder.HasOne(ap => ap.User)
                .WithOne(u => u.AuthorProfile)
                .HasForeignKey<AuthorProfile>(ap => ap.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ap => ap.UserId)
                .IsUnique();

            builder.HasMany(ap => ap.Novels)
                .WithOne(n => n.AuthorProfile)
                .HasForeignKey(n => n.AuthorProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
