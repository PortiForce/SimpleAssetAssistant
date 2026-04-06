using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class TenantRestrictedAssetConfiguration : IEntityTypeConfiguration<TenantRestrictedAsset>
{
	public void Configure(EntityTypeBuilder<TenantRestrictedAsset> builder)
	{
		_ = builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.TenantRestrictedAssetTableName,
			DbConstants.Domain.Entities.CoreSchema.SchemaName);

		_ = builder.HasKey(x => new { x.TenantId, x.AssetId });

		_ = builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From));

		_ = builder.Property(x => x.AssetId)
			.HasConversion(new StrongIdConverter<AssetId>(x => x.Value, AssetId.From));

		_ = builder.HasIndex(x => x.TenantId).HasDatabaseName("IX_TenantRestrictedAsset_TenantId");
	}
}