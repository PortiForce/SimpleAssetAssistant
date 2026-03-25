using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
	public void Configure(EntityTypeBuilder<Account> builder)
	{
		_ = builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.AccountTableName,
			DbConstants.Domain.Entities.CoreSchema.SchemaName);

		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From));

		_ = builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		// Owned ContactInfo
		_ = builder.ComplexProperty(
			x => x.Contact,
			cb =>
			{
				// IMPORTANT: use explicit column names to keep indexes simple & stable
				_ = cb.Property(x => x.Email)
					.HasColumnName("ContactEmail")
					.IsRequired()
					.HasConversion(
						v => v.Value,
						v => Email.Create(v))
					.HasMaxLength(EntityConstraints.CommonSettings.EmailAddressDefaultLength);

				// nullable
				_ = cb.Property(x => x.BackupEmail)
					.HasColumnName("ContactBackupEmail")
					.HasConversion(
						v => v != null ? v.Value : null,
						v => string.IsNullOrEmpty(v) ? null : Email.Create(v))
					.HasMaxLength(EntityConstraints.CommonSettings.EmailAddressDefaultLength);

				// nullable
				_ = cb.Property(x => x.Phone)
					.HasColumnName("ContactPhone")
					.HasConversion(
						v => v != null ? v.Value : null,
						v => string.IsNullOrEmpty(v) ? null : PhoneNumber.Create(v))
					.HasMaxLength(EntityConstraints.Domain.Account.PhoneNumberMaxLength);
			});

		// an alternative to OwnsOne, also NavigationProperty is not needed here
		_ = builder.ComplexProperty(
			x => x.Settings,
			sb =>
			{
				_ = sb.Property(x => x.TimeZoneId)
					.HasColumnName("Settings_TimeZone")
					.HasMaxLength(64);

				_ = sb.Property(x => x.Locale)
					.HasColumnName("Settings_Locale")
					.HasMaxLength(6)
					.IsRequired();

				_ = sb.Property(x => x.DefaultCurrency)
					.HasColumnName("Settings_DefaultFiatCurrency")
					.HasConversion(
						currency => currency.Code,
						value => FiatCurrency.Create(value))
					.HasMaxLength(3);

				_ = sb.Property(x => x.TwoFactorPreferred)
					.HasColumnName("TwoFactorPreferred");
			});

		_ = builder.Property(x => x.Alias)
			.HasMaxLength(EntityConstraints.Domain.Account.AliasMaxLength)
			.IsRequired();

		// RowVersion on mutable aggregate root
		_ = builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		_ = builder.HasIndex(x => x.TenantId)
			.HasDatabaseName("IX_Account_TenantId");

		_ = builder.HasIndex(x => x.State)
			.HasDatabaseName("IX_Account_State");

		// Email unique within tenant
		// should be handled with migration file directly to preserve purity of entity, inside generated Up method
	}
}