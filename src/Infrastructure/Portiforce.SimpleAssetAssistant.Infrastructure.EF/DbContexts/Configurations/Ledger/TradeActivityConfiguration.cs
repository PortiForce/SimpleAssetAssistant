using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Ledger;

public sealed class TradeActivityConfiguration : IEntityTypeConfiguration<TradeActivity>
{
	public void Configure(EntityTypeBuilder<TradeActivity> builder)
	{
		// EF Core knows this is derived from AssetActivityBase, no need to call ToTable() again.

		builder.OwnsOne(x => x.Futures, fb =>
		{
			fb.Property(f => f.InstrumentKey)
				.HasColumnName("Futures_InstrumentKey")
				.HasMaxLength(EntityConstraints.Domain.Activity.FuturesInstrumentKeyLength);

			fb.Property(f => f.ContractKind)
				.HasColumnName("Futures_ContractKind")
				.HasConversion<byte>();

			fb.Property(f => f.BaseAssetCode)
				.HasColumnName("Futures_BaseAssetCode")
				.HasMaxLength(EntityConstraints.Domain.Asset.CodeMaxLength);

			fb.Property(f => f.QuoteAssetCode)
				.HasColumnName("Futures_QuoteAssetCode")
				.HasMaxLength(EntityConstraints.Domain.Asset.CodeMaxLength);

			fb.Property(f => f.PositionEffect)
				.HasColumnName("Futures_PositionEffect")
				.HasConversion<byte>();
		});
	}
}