using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Features.CompetitionStages;

public class CompetitionStageConfiguration : IEntityTypeConfiguration<CompetitionStage>
{
  public void Configure(EntityTypeBuilder<CompetitionStage> builder)
  {
    builder.ToTable("CompetitionStages");

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

    builder.Property(cs => cs.CompetitionSeasonId)
      .HasColumnName("CompetitionSeasonId")
      .IsRequired();

    builder.Property(cs => cs.Name)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(cs => cs.Type)
      .HasConversion<string>()
      .HasMaxLength(20)
      .IsRequired();

    builder.Property(cs => cs.DisplayOrder)
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

    builder.HasOne(cs => cs.CompetitionSeason)
      .WithMany(cs => cs.Stages)
      .HasForeignKey(cs => cs.CompetitionSeasonId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(cs => new { cs.CompetitionSeasonId, cs.Name })
      .IsUnique();

    builder.HasIndex(cs => new { cs.CompetitionSeasonId, cs.DisplayOrder })
      .IsUnique();

    builder.HasIndex(cs => new { cs.CompetitionSeasonId, cs.IsActive });
  }
}
