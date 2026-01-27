using System.Reflection.Emit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;

using Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Ledger;

public sealed class AssetActivityBaseConfiguration : IEntityTypeConfiguration<AssetActivityBase>
{
	public void Configure(EntityTypeBuilder<AssetActivityBase> builder)
	{
		builder.ToTable(
			DbConstants.Domain.Entities.LedgerSchema.ActivityTableName,
			schema: DbConstants.Domain.Entities.LedgerSchema.SchemaName);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<ActivityId>(x => x.Value, ActivityId.From));

		builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		builder.Property(x => x.PlatformAccountId)
			.HasConversion(new StrongIdConverter<PlatformAccountId>(x => x.Value, PlatformAccountId.From))
			.IsRequired();

		builder.Property(x => x.Kind)
			.HasConversion<int>()
			.IsRequired();

		// Discriminator: Kind
		builder.HasDiscriminator(x => x.Kind)
			.HasValue<TradeActivity>(AssetActivityKind.Trade)
			.HasValue<BurnActivity>(AssetActivityKind.Burn)
			.HasValue<IncomeActivity>(AssetActivityKind.Income)
			.HasValue<ServiceActivity>(AssetActivityKind.Service)
			.HasValue<TransferActivity>(AssetActivityKind.Transfer)
			.HasValue<UserCorrectionActivity>(AssetActivityKind.UserCorrection)
			.HasValue<ExchangeActivity>(AssetActivityKind.Exchange);

		// ExternalMetadata moved from OwnsOne to a Complex Property
		builder.ComplexProperty(x => x.ExternalMetadata, meta =>
		{
			meta.Property(m => m.ExternalId)
				.HasColumnName("ExternalId")
				.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

			meta.Property(m => m.Fingerprint)
				.HasColumnName("Fingerprint")
				.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

			meta.Property(m => m.Source)
				.HasColumnName("ExternalSource")
				.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

			meta.Property(m => m.Notes)
				.HasColumnName("ExternalNotes")
				.HasMaxLength(EntityConstraints.CommonSettings.ExternalNotesMaxLength);
		});

		// Facts ordering: OccurredAt then Id, scoped by PlatformAccount
		builder.HasIndex(x => new { x.PlatformAccountId, x.OccurredAt, x.Id })
			.HasDatabaseName("IX_Activity_PlatformAccount_OccurredAt_Id");

		// Idempotency uniqueness (filtered)
		// should be handled with migration file directly to preserve purity of entity, inside generated Up method

		// No RowVersion: Activity should be immutable (facts)
	}
}
