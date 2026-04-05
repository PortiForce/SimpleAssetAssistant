using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Ledger;

public sealed class TransferActivityConfiguration : IEntityTypeConfiguration<TransferActivity>
{
	public void Configure(EntityTypeBuilder<TransferActivity> builder)
	{
		// EF Core knows this is derived from AssetActivityBase, no need to call ToTable() again.

		_ = builder.Property(x => x.Reference)
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

		_ = builder.Property(x => x.Counterparty)
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);
	}
}