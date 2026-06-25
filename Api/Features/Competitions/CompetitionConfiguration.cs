using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Features.Competitions;

public class CompetitionConfiguration : IEntityTypeConfiguration<Competition>
{
  public void Configure(EntityTypeBuilder<Competition> builder)
  {
    builder.ToTable("Competitions");

    builder.HasKey(c => c.Id);

    builder.Property(c => c.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(c => c.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(c => c.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(c => c.Name)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(c => c.Country)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(c => c.LogoUrl)
      .HasMaxLength(500)
      .IsRequired(false);

    builder.Property(c => c.Type)
      .HasConversion<string>()
      .HasMaxLength(20)
      .IsRequired();

    builder.Property(c => c.IsActive)
      .IsRequired();

    builder.HasIndex(c => c.Name)
      .IsUnique();
  }
}
