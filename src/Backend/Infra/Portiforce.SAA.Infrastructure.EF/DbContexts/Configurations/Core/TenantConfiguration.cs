using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
	public void Configure(EntityTypeBuilder<Tenant> builder)
	{
		builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.TenantTableName,
			schema: DbConstants.Domain.Entities.CoreSchema.SchemaName);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From));

		builder.Property(x => x.Name)
			.HasMaxLength(EntityConstraints.Domain.Tenant.NameMaxLength)
			.IsRequired();

		builder.Property(x => x.Code)
			.HasMaxLength(EntityConstraints.CommonSettings.CodeMaxLength)
			.IsRequired();

		builder.Property(x => x.BrandName)
			.HasMaxLength(EntityConstraints.Domain.Tenant.BrandMinLength);

		builder.Property(x => x.DomainPrefix)
			.HasMaxLength(EntityConstraints.Domain.Tenant.PublicDomainMaxLength);

		builder.Property(x => x.Plan)
			.IsRequired()
			.HasConversion<byte>();

		builder.Property(x => x.State)
			.IsRequired()
			.HasConversion<byte>();

		// ============================
		// TenantSettings (owned root)
		// ============================
		builder.ComplexProperty(x => x.Settings, sb =>
		{
			// ---------- Defaults ----------
			sb.ComplexProperty(x => x.Defaults, db =>
			{
				db.Property(x => x.DefaultCurrency)
					.HasColumnName("Defaults_DefaultCurrency")
					.HasConversion(
						currency => currency.Code,
						value => FiatCurrency.Create(value)
					)
					.HasMaxLength(3)
					.IsRequired();

				db.Property(x => x.DefaultTimeZoneId)
					.HasColumnName("Defaults_DefaultTimeZone")
					.HasMaxLength(64)
					.IsRequired();

				db.Property(x => x.DefaultLocale)
					.HasColumnName("Defaults_DefaultLocale")
					.HasMaxLength(6)
					.IsRequired();
			});

			// ---------- Security ----------
			sb.ComplexProperty(x => x.Security, sec =>
			{
				sec.Property(x => x.EnforceTwoFactor)
					.HasColumnName("Security_EnforceTwoFactor")
					.IsRequired();
			});

			// ---------- Import ----------
			sb.ComplexProperty(x => x.Import, imp =>
			{
				imp.Property(x => x.RequireReviewBeforeProcessing)
					.HasColumnName("Import_RequireReview")
					.IsRequired();

				imp.Property(x => x.MaxRowsPerImport)
					.HasColumnName("Import_MaxRows")
					.IsRequired();

				imp.Property(x => x.MaxFileSizeMb)
					.HasColumnName("Import_MaxFileSizeMb")
					.IsRequired();
			});

			// ---------- Retention ----------
			sb.ComplexProperty(x => x.Retention, ret =>
			{
				ret.Property(x => x.DeletedDataRetentionDays)
					.HasColumnName("Retention_DeletedDays")
					.IsRequired();
			});
		});

		// RowVersion on mutable aggregate root
		builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		// Indexes
		builder.HasIndex(x => x.State).HasDatabaseName("IX_Tenant_State");

		builder.HasIndex(x => x.Code)
			.IsUnique()
			.HasDatabaseName("UX_Tenant_Code");

		// Domain prefix: unique when not null
		builder.HasIndex(x => x.DomainPrefix)
			.IsUnique()
			.HasDatabaseName("UX_Tenant_DomainPrefix")
			.HasFilter("[DomainPrefix] IS NOT NULL");

		builder.Navigation(x => x.RestrictedAssets).AutoInclude(false);
		builder.Navigation(x => x.RestrictedPlatforms).AutoInclude(false);
	}
}
