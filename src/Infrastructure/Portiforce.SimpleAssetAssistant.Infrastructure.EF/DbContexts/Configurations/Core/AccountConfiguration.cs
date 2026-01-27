using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;

using Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
	public void Configure(EntityTypeBuilder<Account> builder)
	{
		builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.AccountTableName,
			schema: DbConstants.Domain.Entities.CoreSchema.SchemaName);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From));

		builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		// Owned ContactInfo
		builder.ComplexProperty(x => x.Contact, cb =>
		{
			// IMPORTANT: use explicit column names to keep indexes simple & stable
			cb.Property(x => x.Email)
				.HasColumnName("ContactEmail")
				.IsRequired()
				.HasConversion(
					v => v.Value,
					v => Email.Create(v))
				.HasMaxLength(EntityConstraints.CommonSettings.EmailAddressDefaultLength);

			// nullable
			cb.Property(x => x.BackupEmail)
				.HasColumnName("ContactBackupEmail")
				.HasConversion(
					v => v != null ? v.Value : null,
					v => string.IsNullOrEmpty(v) ? null : Email.Create(v))
				.HasMaxLength(EntityConstraints.CommonSettings.EmailAddressDefaultLength);

			// nullable
			cb.Property(x => x.Phone)
				.HasColumnName("ContactPhone")
				.HasConversion(
					v => v != null ? v.Value : null,
					v => string.IsNullOrEmpty(v) ? null : PhoneNumber.Create(v))
				.HasMaxLength(EntityConstraints.Domain.Account.PhoneNumberMaxLength);
		});

		// an alternative to OwnsOne, also NavigationProperty is not needed here
		builder.ComplexProperty(x => x.Settings, sb =>
		{
			sb.Property(x => x.TimeZoneId)
				.HasColumnName("Settings_TimeZone")
				.HasMaxLength(64);

			sb.Property(x => x.Locale)
				.HasColumnName("Settings_Locale")
				.HasMaxLength(6)
				.IsRequired();

			sb.Property(x => x.DefaultCurrency)
				.HasColumnName("Settings_DefaultFiatCurrency")
				.HasConversion(
					currency => currency.Code,
					value => FiatCurrency.Create(value)
				)
				.HasMaxLength(3);

			sb.Property(x => x.TwoFactorPreferred)
				.HasColumnName("TwoFactorPreferred");
		});

		builder.Property(x => x.Alias)
			.HasMaxLength(EntityConstraints.Domain.Account.AliasMaxLength)
			.IsRequired();

		// RowVersion on mutable aggregate root
		builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		builder.HasIndex(x => x.TenantId)
			.HasDatabaseName("IX_Account_TenantId");

		builder.HasIndex(x => x.State)
			.HasDatabaseName("IX_Account_State");

		// Email unique within tenant
		// should be handled with migration file directly to preserve purity of entity, inside generated Up method
	}
}
