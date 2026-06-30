using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Features.CompetitionTeams;

public class CompetitionTeamConfiguration : IEntityTypeConfiguration<CompetitionTeam>
{
  public void Configure(EntityTypeBuilder<CompetitionTeam> builder)
  {
    builder.ToTable("CompetitionTeams");

    builder.HasKey(ct => ct.Id);

    builder.Property(ct => ct.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(ct => ct.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(ct => ct.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(ct => ct.CompetitionSeasonId)
      .HasColumnName("CompetitionSeasonId")
      .IsRequired();

    builder.Property(ct => ct.TeamId)
      .HasColumnName("TeamId")
      .IsRequired();

    builder.Property(ct => ct.CompetitionStageId)
      .HasColumnName("CompetitionStageId")
      .IsRequired(false);

    builder.Property(ct => ct.CompetitionGroupId)
      .HasColumnName("CompetitionGroupId")
      .IsRequired(false);

    builder.Property(ct => ct.Seed)
      .IsRequired(false);

    builder.Property(ct => ct.IsActive)
      .IsRequired();

    builder.HasOne(ct => ct.CompetitionSeason)
      .WithMany(cs => cs.CompetitionTeams)
      .HasForeignKey(ct => ct.CompetitionSeasonId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(ct => ct.Team)
      .WithMany(t => t.CompetitionTeams)
      .HasForeignKey(ct => ct.TeamId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(ct => ct.CompetitionStage)
      .WithMany(cs => cs.CompetitionTeams)
      .HasForeignKey(ct => ct.CompetitionStageId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(ct => ct.CompetitionGroup)
      .WithMany(cg => cg.CompetitionTeams)
      .HasForeignKey(ct => ct.CompetitionGroupId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(ct => new { ct.CompetitionSeasonId, ct.TeamId })
      .IsUnique();

    builder.HasIndex(ct => new { ct.CompetitionSeasonId, ct.IsActive });
    builder.HasIndex(ct => new { ct.CompetitionStageId, ct.IsActive });
    builder.HasIndex(ct => new { ct.CompetitionGroupId, ct.IsActive });
  }
}
