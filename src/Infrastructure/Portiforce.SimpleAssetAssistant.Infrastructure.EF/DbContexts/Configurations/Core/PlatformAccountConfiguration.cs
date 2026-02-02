using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Assets.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class PlatformAccountConfiguration : IEntityTypeConfiguration<PlatformAccount>
{
	public void Configure(EntityTypeBuilder<PlatformAccount> builder)
	{
		builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.PlatformAccountTableName,
			schema: DbConstants.Domain.Entities.CoreSchema.SchemaName);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<PlatformAccountId>(x => x.Value, PlatformAccountId.From));

		builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		builder.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		builder.Property(x => x.PlatformId)
			.HasConversion(new StrongIdConverter<PlatformId>(x => x.Value, PlatformId.From))
			.IsRequired();

		builder.Property(x => x.AccountName)
			.HasMaxLength(EntityConstraints.Domain.PlatformAccount.NameMaxLength)
			.IsRequired();

		builder.Property(x => x.ExternalUserId)
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

		builder.Property(x => x.ExternalAccountId)
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

		builder.Property(x => x.State)
			.IsRequired()
			.HasConversion<byte>();

		// RowVersion: user might edit mapping/settings
		builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		builder.HasIndex(x => new { x.TenantId, x.AccountId })
			.HasDatabaseName("IX_PlatformAccount_Tenant_Account");

		builder.HasIndex(x => new { x.TenantId, x.PlatformId })
			.HasDatabaseName("IX_PlatformAccount_Tenant_Platform");

		// Choose your uniqueness rule. This is a common MVP one:
		builder.HasIndex(x => new { x.TenantId, x.AccountId, x.PlatformId })
			.IsUnique()
			.HasDatabaseName("UX_PlatformAccount_Tenant_Account_Platform");
	}
}
