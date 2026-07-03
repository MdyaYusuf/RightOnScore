using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Features.Matches;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
  public void Configure(EntityTypeBuilder<Match> builder)
  {
    builder.ToTable("Matches");

    builder.HasKey(m => m.Id);

    builder.Property(m => m.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(m => m.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(m => m.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(m => m.CompetitionSeasonId)
      .HasColumnName("CompetitionSeasonId")
      .IsRequired();

    builder.Property(m => m.CompetitionStageId)
      .HasColumnName("CompetitionStageId")
      .IsRequired(false);

    builder.Property(m => m.CompetitionGroupId)
      .HasColumnName("CompetitionGroupId")
      .IsRequired(false);

    builder.Property(m => m.HomeTeamId)
      .HasColumnName("HomeTeamId")
      .IsRequired();

    builder.Property(m => m.AwayTeamId)
      .HasColumnName("AwayTeamId")
      .IsRequired();

    builder.Property(m => m.KickoffTime)
      .IsRequired();

    builder.Property(m => m.Status)
      .HasConversion<string>()
      .HasMaxLength(20)
      .IsRequired();

    builder.Property(m => m.Round)
      .IsRequired(false);

    builder.Property(m => m.Venue)
      .HasMaxLength(200)
      .IsRequired(false);

    builder.Property(m => m.HomeScore)
      .IsRequired(false);

    builder.Property(m => m.AwayScore)
      .IsRequired(false);

    builder.HasOne(m => m.CompetitionSeason)
      .WithMany(cs => cs.Matches)
      .HasForeignKey(m => m.CompetitionSeasonId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(m => m.CompetitionStage)
      .WithMany(cs => cs.Matches)
      .HasForeignKey(m => m.CompetitionStageId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(m => m.CompetitionGroup)
      .WithMany(cg => cg.Matches)
      .HasForeignKey(m => m.CompetitionGroupId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(m => m.HomeTeam)
      .WithMany()
      .HasForeignKey(m => m.HomeTeamId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(m => m.AwayTeam)
      .WithMany()
      .HasForeignKey(m => m.AwayTeamId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(m => new { m.CompetitionSeasonId, m.KickoffTime });
    builder.HasIndex(m => new { m.CompetitionStageId, m.KickoffTime });
    builder.HasIndex(m => new { m.CompetitionGroupId, m.KickoffTime });
    builder.HasIndex(m => m.Status);
  }
}
