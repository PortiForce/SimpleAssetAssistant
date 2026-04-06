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
		_ = builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.TenantTableName,
			DbConstants.Domain.Entities.CoreSchema.SchemaName);

		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From));

		_ = builder.Property(x => x.Name)
			.HasMaxLength(EntityConstraints.Domain.Tenant.NameMaxLength)
			.IsRequired();

		_ = builder.Property(x => x.Code)
			.HasMaxLength(EntityConstraints.CommonSettings.CodeMaxLength)
			.IsRequired();

		_ = builder.Property(x => x.BrandName)
			.HasMaxLength(EntityConstraints.Domain.Tenant.BrandMinLength);

		_ = builder.Property(x => x.DomainPrefix)
			.HasMaxLength(EntityConstraints.Domain.Tenant.PublicDomainMaxLength);

		_ = builder.Property(x => x.Plan)
			.IsRequired()
			.HasConversion<byte>();

		_ = builder.Property(x => x.State)
			.IsRequired()
			.HasConversion<byte>();

		// ============================
		// TenantSettings (owned root)
		// ============================
		_ = builder.ComplexProperty(
			x => x.Settings,
			sb =>
			{
				// ---------- Defaults ----------
				_ = sb.ComplexProperty(
					x => x.Defaults,
					db =>
					{
						_ = db.Property(x => x.DefaultCurrency)
							.HasColumnName("Defaults_DefaultCurrency")
							.HasConversion(
								currency => currency.Code,
								value => FiatCurrency.Create(value))
							.HasMaxLength(3)
							.IsRequired();

						_ = db.Property(x => x.DefaultTimeZoneId)
							.HasColumnName("Defaults_DefaultTimeZone")
							.HasMaxLength(64)
							.IsRequired();

						_ = db.Property(x => x.DefaultLocale)
							.HasColumnName("Defaults_DefaultLocale")
							.HasMaxLength(6)
							.IsRequired();
					});

				// ---------- Security ----------
				_ = sb.ComplexProperty(
					x => x.Security,
					sec =>
					{
						_ = sec.Property(x => x.EnforceTwoFactor)
							.HasColumnName("Security_EnforceTwoFactor")
							.IsRequired();
					});

				// ---------- Import ----------
				_ = sb.ComplexProperty(
					x => x.Import,
					imp =>
					{
						_ = imp.Property(x => x.RequireReviewBeforeProcessing)
							.HasColumnName("Import_RequireReview")
							.IsRequired();

						_ = imp.Property(x => x.MaxRowsPerImport)
							.HasColumnName("Import_MaxRows")
							.IsRequired();

						_ = imp.Property(x => x.MaxFileSizeMb)
							.HasColumnName("Import_MaxFileSizeMb")
							.IsRequired();
					});

				// ---------- Retention ----------
				_ = sb.ComplexProperty(
					x => x.Retention,
					ret =>
					{
						_ = ret.Property(x => x.DeletedDataRetentionDays)
							.HasColumnName("Retention_DeletedDays")
							.IsRequired();
					});
			});

		// RowVersion on mutable aggregate root
		_ = builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		// Indexes
		_ = builder.HasIndex(x => x.State).HasDatabaseName("IX_Tenant_State");

		_ = builder.HasIndex(x => x.Code)
			.IsUnique()
			.HasDatabaseName("UX_Tenant_Code");

		// Domain prefix: unique when not null
		_ = builder.HasIndex(x => x.DomainPrefix)
			.IsUnique()
			.HasDatabaseName("UX_Tenant_DomainPrefix")
			.HasFilter("[DomainPrefix] IS NOT NULL");

		_ = builder.Navigation(x => x.RestrictedAssets).AutoInclude(false);
		_ = builder.Navigation(x => x.RestrictedPlatforms).AutoInclude(false);
	}
}