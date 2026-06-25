using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Features.CompetitionSeasons;

public class CompetitionSeasonConfiguration : IEntityTypeConfiguration<CompetitionSeason>
{
  public void Configure(EntityTypeBuilder<CompetitionSeason> builder)
  {
    builder.ToTable("CompetitionSeasons");

    builder.HasKey(cs => cs.Id);

    builder.Property(cs => cs.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(cs => cs.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(cs => cs.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(cs => cs.CompetitionId)
      .HasColumnName("CompetitionId")
      .IsRequired();

    builder.Property(cs => cs.Name)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(cs => cs.StartDate)
      .IsRequired();

    builder.Property(cs => cs.EndDate)
      .IsRequired();

    builder.Property(cs => cs.Status)
      .HasConversion<string>()
      .HasMaxLength(20)
      .IsRequired();

    builder.Property(cs => cs.IsActive)
      .IsRequired();

    builder.HasOne(cs => cs.Competition)
      .WithMany(c => c.Seasons)
      .HasForeignKey(cs => cs.CompetitionId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(cs => new { cs.CompetitionId, cs.Name })
      .IsUnique();

    builder.HasIndex(cs => new { cs.CompetitionId, cs.IsActive });
  }
}
