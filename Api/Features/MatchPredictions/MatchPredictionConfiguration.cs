using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Features.MatchPredictions;

public class MatchPredictionConfiguration : IEntityTypeConfiguration<MatchPrediction>
{
  public void Configure(EntityTypeBuilder<MatchPrediction> builder)
  {
    builder.ToTable("MatchPredictions");

    builder.HasKey(p => p.Id);

    builder.Property(p => p.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(p => p.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(p => p.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(p => p.UserId)
      .HasColumnName("UserId")
      .IsRequired();

    builder.Property(p => p.MatchId)
      .HasColumnName("MatchId")
      .IsRequired();

    builder.Property(p => p.PredictedHomeScore)
      .IsRequired();

    builder.Property(p => p.PredictedAwayScore)
      .IsRequired();

    builder.Property(p => p.PredictedAdvancingTeamId)
      .IsRequired(false);

    builder.Property(p => p.PointsEarned)
      .IsRequired(false);

    builder.HasOne(p => p.User)
      .WithMany(u => u.MatchPredictions)
      .HasForeignKey(p => p.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(p => p.Match)
      .WithMany(m => m.MatchPredictions)
      .HasForeignKey(p => p.MatchId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(p => p.PredictedAdvancingTeam)
      .WithMany()
      .HasForeignKey(p => p.PredictedAdvancingTeamId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(p => new { p.UserId, p.MatchId })
      .IsUnique();

    builder.HasIndex(p => p.MatchId);
    builder.HasIndex(p => new { p.UserId, p.PointsEarned });
  }
}
