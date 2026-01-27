using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Ledger;

public sealed class TransferActivityConfiguration : IEntityTypeConfiguration<TransferActivity>
{
	public void Configure(EntityTypeBuilder<TransferActivity> builder)
	{
		// EF Core knows this is derived from AssetActivityBase, no need to call ToTable() again.

		builder.Property(x => x.Reference)
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

		builder.Property(x => x.Counterparty)
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);
	}
}
