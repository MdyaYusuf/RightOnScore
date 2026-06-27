using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Features.Teams;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
  public void Configure(EntityTypeBuilder<Team> builder)
  {
    builder.ToTable("Teams");

    builder.HasKey(t => t.Id);

    builder.Property(t => t.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(t => t.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(t => t.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(t => t.Name)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(t => t.ShortName)
      .HasMaxLength(30)
      .IsRequired();

    builder.Property(t => t.Country)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(t => t.CrestUrl)
      .HasMaxLength(500)
      .IsRequired(false);

    builder.Property(t => t.IsActive)
      .IsRequired();

    builder.HasIndex(t => new { t.Name, t.Country })
      .IsUnique();

    builder.HasIndex(t => new { t.ShortName, t.Country })
      .IsUnique();

    builder.HasIndex(t => t.IsActive);
  }
}
