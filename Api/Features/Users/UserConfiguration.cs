using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Features.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("Users");

    builder.HasKey(u => u.Id);

    builder.Property(u => u.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(u => u.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(u => u.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(u => u.Username)
      .HasMaxLength(50)
      .IsRequired();

    builder.Property(u => u.Email)
      .HasMaxLength(150)
      .IsRequired();

    builder.Property(u => u.PasswordHash)
      .IsRequired();

    builder.Property(u => u.PasswordKey)
      .IsRequired();

    builder.Property(u => u.RefreshToken)
      .HasMaxLength(500)
      .IsRequired(false);

    builder.Property(u => u.RefreshTokenExpiration)
      .IsRequired(false);

    builder.Property(u => u.ProfileImageUrl)
      .HasMaxLength(500)
      .IsRequired(false);

    builder.Property(u => u.IsActive)
      .HasDefaultValue(true)
      .IsRequired();

    builder.Property(u => u.RoleId)
      .IsRequired();

    builder.HasOne(u => u.Role)
      .WithMany(r => r.Users)
      .HasForeignKey(u => u.RoleId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(u => u.Username)
      .IsUnique();

    builder.HasIndex(u => u.Email)
      .IsUnique();
  }
}