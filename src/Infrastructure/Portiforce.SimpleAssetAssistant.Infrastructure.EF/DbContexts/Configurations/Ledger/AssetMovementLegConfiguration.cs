using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Ledger;

public sealed class AssetMovementLegConfiguration : IEntityTypeConfiguration<AssetMovementLeg>
{
	public void Configure(EntityTypeBuilder<AssetMovementLeg> builder)
	{
		builder.ToTable(
			DbConstants.Domain.Entities.LedgerSchema.ActivityLegTableName,
			schema: DbConstants.Domain.Entities.LedgerSchema.SchemaName);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<LegId>(x => x.Value, LegId.From));

		builder.Property(x => x.ActivityId)
			.HasConversion(new StrongIdConverter<ActivityId>(x => x.Value, ActivityId.From))
			.IsRequired();

		builder.Property(x => x.AssetId)
			.HasConversion(new StrongIdConverter<AssetId>(x => x.Value, AssetId.From))
			.IsRequired();

		builder.Property(x => x.Direction)
			.IsRequired()
			.HasConversion<byte>();

		builder.Property(x => x.Role)
			.IsRequired()
			.HasConversion<byte>();

		builder.Property(x => x.Allocation)
			.IsRequired()
			.HasConversion<byte>();

		// Quantity is VO: map to decimal
		builder.Property(x => x.Amount)
			.HasConversion(
				v => v.Value,
				v => Quantity.Create(v)
			)
			.HasColumnType("decimal(38, 18)")
			.IsRequired();

		builder.Property(x => x.InstrumentKey)
			.HasMaxLength(EntityConstraints.Domain.ActivityLeg.InstrumentKeyMaxLength);

		builder.HasIndex(x => x.ActivityId)
			.HasDatabaseName("IX_Leg_ActivityId");

		builder.HasIndex(x => x.AssetId)
			.HasDatabaseName("IX_Leg_AssetId");

		// No RowVersion: as legs are immutable if activity is immutable
	}
}
