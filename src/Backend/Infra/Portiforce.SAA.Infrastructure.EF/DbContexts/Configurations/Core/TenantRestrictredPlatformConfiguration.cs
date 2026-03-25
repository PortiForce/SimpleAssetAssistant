using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class TenantRestrictedPlatformConfiguration : IEntityTypeConfiguration<TenantRestrictedPlatform>
{
	public void Configure(EntityTypeBuilder<TenantRestrictedPlatform> builder)
	{
		_ = builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.TenantRestrictedPlatformTableName,
			DbConstants.Domain.Entities.CoreSchema.SchemaName);

		_ = builder.HasKey(x => new { x.TenantId, x.PlatformId });

		_ = builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From));

		_ = builder.Property(x => x.PlatformId)
			.HasConversion(new StrongIdConverter<PlatformId>(x => x.Value, PlatformId.From));

		_ = builder.HasIndex(x => x.TenantId).HasDatabaseName("IX_TenantRestrictedPlatform_TenantId");
	}
}