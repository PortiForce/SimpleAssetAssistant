using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class PlatformAccountConfiguration : IEntityTypeConfiguration<PlatformAccount>
{
	public void Configure(EntityTypeBuilder<PlatformAccount> builder)
	{
		_ = builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.PlatformAccountTableName,
			DbConstants.Domain.Entities.CoreSchema.SchemaName);

		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<PlatformAccountId>(x => x.Value, PlatformAccountId.From));

		_ = builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		_ = builder.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		_ = builder.Property(x => x.PlatformId)
			.HasConversion(new StrongIdConverter<PlatformId>(x => x.Value, PlatformId.From))
			.IsRequired();

		_ = builder.Property(x => x.AccountName)
			.HasMaxLength(EntityConstraints.Domain.PlatformAccount.NameMaxLength)
			.IsRequired();

		_ = builder.Property(x => x.ExternalUserId)
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

		_ = builder.Property(x => x.ExternalAccountId)
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

		_ = builder.Property(x => x.State)
			.IsRequired()
			.HasConversion<byte>();

		// RowVersion: user might edit mapping/settings
		_ = builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		_ = builder.HasIndex(x => new { x.TenantId, x.AccountId })
			.HasDatabaseName("IX_PlatformAccount_Tenant_Account");

		_ = builder.HasIndex(x => new { x.TenantId, x.PlatformId })
			.HasDatabaseName("IX_PlatformAccount_Tenant_Platform");

		// Choose your uniqueness rule. This is a common MVP one:
		_ = builder.HasIndex(x => new { x.TenantId, x.AccountId, x.PlatformId })
			.IsUnique()
			.HasDatabaseName("UX_PlatformAccount_Tenant_Account_Platform");
	}
}