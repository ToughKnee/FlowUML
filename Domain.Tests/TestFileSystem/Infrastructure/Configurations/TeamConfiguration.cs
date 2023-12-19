using CleanArchitectureWorkshop.Domain.TeamAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitectureWorkshop.Infrastructure.Configurations;

public class TeamConfigurations : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(
                convertToProviderExpression: id => id.Value,
                convertFromProviderExpression: id => TeamName.Create(id))
            .HasColumnName("Name");
        builder.HasMany(t => t.Players)
            .WithOne(p => p.Team)
            .HasForeignKey("TeamName");

    }
}
