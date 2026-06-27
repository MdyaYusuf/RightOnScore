using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Features.CompetitionGroups;

public class CompetitionGroupConfiguration : IEntityTypeConfiguration<CompetitionGroup>
{
  public void Configure(EntityTypeBuilder<CompetitionGroup> builder)
  {
    builder.ToTable("CompetitionGroups");

    builder.HasKey(cg => cg.Id);

    builder.Property(cg => cg.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(cg => cg.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(cg => cg.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(cg => cg.CompetitionStageId)
      .HasColumnName("CompetitionStageId")
      .IsRequired();

    builder.Property(cg => cg.Name)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(cg => cg.DisplayOrder)
      .IsRequired();

    builder.Property(cg => cg.IsActive)
      .IsRequired();

    builder.HasOne(cg => cg.CompetitionStage)
      .WithMany(cs => cs.Groups)
      .HasForeignKey(cg => cg.CompetitionStageId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(cg => new { cg.CompetitionStageId, cg.Name })
      .IsUnique();

    builder.HasIndex(cg => new { cg.CompetitionStageId, cg.DisplayOrder })
      .IsUnique();

    builder.HasIndex(cg => new { cg.CompetitionStageId, cg.IsActive });
  }
}
