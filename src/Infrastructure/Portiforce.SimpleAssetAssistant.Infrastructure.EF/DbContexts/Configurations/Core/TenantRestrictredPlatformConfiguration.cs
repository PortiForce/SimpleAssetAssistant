using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;

using Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class TenantRestrictedPlatformConfiguration : IEntityTypeConfiguration<TenantRestrictedPlatform>
{
	public void Configure(EntityTypeBuilder<TenantRestrictedPlatform> builder)
	{
		builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.TenantRestrictedPlatformTableName,
			schema: DbConstants.Domain.Entities.CoreSchema.SchemaName);

		builder.HasKey(x => new { x.TenantId, x.PlatformId });

		builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From));

		builder.Property(x => x.PlatformId)
			.HasConversion(new StrongIdConverter<PlatformId>(x => x.Value, PlatformId.From));

		builder.HasIndex(x => x.TenantId).HasDatabaseName("IX_TenantRestrictedPlatform_TenantId");
	}
}