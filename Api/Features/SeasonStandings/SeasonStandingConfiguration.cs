using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Features.SeasonStandings;

public class SeasonStandingConfiguration : IEntityTypeConfiguration<SeasonStanding>
{
  public void Configure(EntityTypeBuilder<SeasonStanding> builder)
  {
    builder.ToTable("SeasonStandings");

    builder.HasKey(s => s.Id);

    builder.Property(s => s.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(s => s.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(s => s.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(s => s.UserId)
      .HasColumnName("UserId")
      .IsRequired();

    builder.Property(s => s.CompetitionSeasonId)
      .HasColumnName("CompetitionSeasonId")
      .IsRequired();

    builder.Property(s => s.TotalPoints)
      .IsRequired();

    builder.Property(s => s.ExactScoreCount)
      .IsRequired();

    builder.Property(s => s.ScoredPredictionCount)
      .IsRequired();

    builder.HasOne(s => s.User)
      .WithMany(u => u.SeasonStandings)
      .HasForeignKey(s => s.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(s => s.CompetitionSeason)
      .WithMany(cs => cs.SeasonStandings)
      .HasForeignKey(s => s.CompetitionSeasonId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(s => new { s.UserId, s.CompetitionSeasonId })
      .IsUnique();

    builder.HasIndex(s => new { s.CompetitionSeasonId, s.TotalPoints, s.ExactScoreCount });
  }
}
