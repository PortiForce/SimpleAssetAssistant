using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Ledger;

public sealed class AssetMovementLegConfiguration : IEntityTypeConfiguration<AssetMovementLeg>
{
	public void Configure(EntityTypeBuilder<AssetMovementLeg> builder)
	{
		_ = builder.ToTable(
			DbConstants.Domain.Entities.LedgerSchema.ActivityLegTableName,
			DbConstants.Domain.Entities.LedgerSchema.SchemaName);

		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<LegId>(x => x.Value, LegId.From));

		_ = builder.Property(x => x.ActivityId)
			.HasConversion(new StrongIdConverter<ActivityId>(x => x.Value, ActivityId.From))
			.IsRequired();

		_ = builder.Property(x => x.AssetId)
			.HasConversion(new StrongIdConverter<AssetId>(x => x.Value, AssetId.From))
			.IsRequired();

		_ = builder.Property(x => x.Direction)
			.IsRequired()
			.HasConversion<byte>();

		_ = builder.Property(x => x.Role)
			.IsRequired()
			.HasConversion<byte>();

		_ = builder.Property(x => x.Allocation)
			.IsRequired()
			.HasConversion<byte>();

		// Quantity is VO: map to decimal
		_ = builder.Property(x => x.Amount)
			.HasConversion(
				v => v.Value,
				v => Quantity.Create(v))
			.HasColumnType("decimal(38, 18)")
			.IsRequired();

		_ = builder.Property(x => x.InstrumentKey)
			.HasMaxLength(EntityConstraints.Domain.ActivityLeg.InstrumentKeyMaxLength);

		_ = builder.HasIndex(x => x.ActivityId)
			.HasDatabaseName("IX_Leg_ActivityId");

		_ = builder.HasIndex(x => x.AssetId)
			.HasDatabaseName("IX_Leg_AssetId");

		// No RowVersion: as legs are immutable if activity is immutable
	}
}