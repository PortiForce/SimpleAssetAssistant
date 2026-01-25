using Microsoft.EntityFrameworkCore;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Assets.Models;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DataPopulation.Seeders;
using Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

public class AssetAssistantDbContext(DbContextOptions<AssetAssistantDbContext> options) : DbContext(options)
{
	public DbSet<Tenant> Tenants => Set<Tenant>();
	public DbSet<Account> Accounts => Set<Account>();

	public DbSet<Platform> Platforms => Set<Platform>();
	public DbSet<PlatformAccount> PlatformAccounts => Set<PlatformAccount>();

	public DbSet<Asset> Assets => Set<Asset>();

	public DbSet<AssetActivityBase> Activities => Set<AssetActivityBase>();
	public DbSet<AssetMovementLeg> ActivityLegs => Set<AssetMovementLeg>();

	// Join tables (recommended to expose for admin flows / diagnostics)
	public DbSet<TenantRestrictedAsset> TenantRestrictedAssets => Set<TenantRestrictedAsset>();
	public DbSet<TenantRestrictedPlatform> TenantRestrictedPlatforms => Set<TenantRestrictedPlatform>();

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		// Only apply if not configured by DI (e.g. design-time tools / tests)
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.EnableDetailedErrors();
#if DEBUG
			// ONLY for local debugging
			optionsBuilder.EnableSensitiveDataLogging();
#endif
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(DbConstants.Domain.DefaultSchemaName);

		DefineTenantConfiguration(modelBuilder);
		DefineAccountConfiguration(modelBuilder);

		DefinePlatformConfiguration(modelBuilder);
		DefinePlatformAccountConfiguration(modelBuilder);

		DefineAssetConfiguration(modelBuilder);

		DefineActivityConfiguration(modelBuilder);
		DefineActivityLegsConfiguration(modelBuilder);

		DefineRestrictionJoinTables(modelBuilder);

		ConfigureRelationships(modelBuilder);
	}

	private static void DefineTenantConfiguration(ModelBuilder modelBuilder)
	{
		var e = modelBuilder.Entity<Tenant>();
		e.ToTable(DbConstants.Domain.EntityNames.TenantEntityName);

		e.HasKey(x => x.Id);

		e.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From));

		e.Property(x => x.Name)
			.HasMaxLength(EntityConstraints.Domain.Tenant.NameMaxLength)
			.IsRequired();

		e.Property(x => x.Code)
			.HasMaxLength(EntityConstraints.CommonSettings.CodeMaxLength)
			.IsRequired();

		e.Property(x => x.BrandName)
			.HasMaxLength(EntityConstraints.Domain.Tenant.BrandMinLength);

		e.Property(x => x.DomainPrefix)
			.HasMaxLength(EntityConstraints.Domain.Tenant.PublicDomainMaxLength);

		e.Property(x => x.Plan)
			.IsRequired()
			.HasConversion<int>();

		e.Property(x => x.State)
			.IsRequired()
			.HasConversion<int>();

		// ============================
		// TenantSettings (owned root)
		// ============================
		e.ComplexProperty(x => x.Settings, sb =>
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
		e.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		// Indexes
		e.HasIndex(x => x.State).HasDatabaseName("IX_Tenant_State");

		e.HasIndex(x => x.Code)
			.IsUnique()
			.HasDatabaseName("UX_Tenant_Code");

		// Domain prefix: unique when not null
		e.HasIndex(x => x.DomainPrefix)
			.IsUnique()
			.HasDatabaseName("UX_Tenant_DomainPrefix")
			.HasFilter("[DomainPrefix] IS NOT NULL");

		// todo tech: might be an issue
		e.Navigation(x => x.RestrictedAssets).AutoInclude(false);
		e.Navigation(x => x.RestrictedPlatforms).AutoInclude(false);
	}

	private static void DefineAccountConfiguration(ModelBuilder modelBuilder)
	{
		var e = modelBuilder.Entity<Account>();
		e.ToTable(DbConstants.Domain.EntityNames.AccountEntityName);

		e.HasKey(x => x.Id);

		e.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From));

		e.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		// Owned ContactInfo
		e.ComplexProperty(x => x.Contact, cb =>
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
		e.ComplexProperty(x => x.Settings, sb =>
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
		
		e.Property(x => x.Alias)
			.HasMaxLength(EntityConstraints.Domain.Account.AliasMaxLength)
			.IsRequired();

		// RowVersion on mutable aggregate root
		e.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		e.HasIndex(x => x.TenantId)
			.HasDatabaseName("IX_Account_TenantId");

		e.HasIndex(x => x.State)
			.HasDatabaseName("IX_Account_State");

		// Email unique within tenant
		// should be handled with migration file directly to preserve purity of entity, inside generated Up method
	}

	private static void DefinePlatformConfiguration(ModelBuilder modelBuilder)
	{
		var e = modelBuilder.Entity<Platform>();
		e.ToTable(DbConstants.Domain.EntityNames.PlatformEntityName);

		e.HasKey(x => x.Id);

		e.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<PlatformId>(x => x.Value, PlatformId.From));

		e.Property(x => x.Code)
			.HasMaxLength(EntityConstraints.CommonSettings.CodeMaxLength)
			.IsRequired();

		e.Property(x => x.Name)
			.HasMaxLength(EntityConstraints.Domain.Platform.NameMaxLength)
			.IsRequired();

		e.Property(x => x.Kind)
			.IsRequired()
			.HasConversion<int>();

		e.Property(x => x.State)
			.IsRequired()
			.HasConversion<int>();

		e.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		e.HasIndex(x => x.Code)
			.IsUnique()
			.HasDatabaseName("UX_Platform_Code");

		e.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UX_Platform_Name");

		e.HasIndex(x => x.State)
			.HasDatabaseName("IX_Platform_State");
	}

	private static void DefinePlatformAccountConfiguration(ModelBuilder modelBuilder)
	{
		var e = modelBuilder.Entity<PlatformAccount>();
		e.ToTable(DbConstants.Domain.EntityNames.PlatformAccountEntityName);

		e.HasKey(x => x.Id);

		e.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<PlatformAccountId>(x => x.Value, PlatformAccountId.From));

		e.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		e.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		e.Property(x => x.PlatformId)
			.HasConversion(new StrongIdConverter<PlatformId>(x => x.Value, PlatformId.From))
			.IsRequired();

		e.Property(x => x.AccountName)
			.HasMaxLength(EntityConstraints.Domain.PlatformAccount.NameMaxLength)
			.IsRequired();

		e.Property(x => x.ExternalUserId)
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

		e.Property(x => x.ExternalAccountId)
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

		e.Property(x => x.State)
			.IsRequired()
			.HasConversion<int>();

		// RowVersion: user might edit mapping/settings
		e.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		e.HasIndex(x => new { x.TenantId, x.AccountId })
			.HasDatabaseName("IX_PlatformAccount_Tenant_Account");

		e.HasIndex(x => new { x.TenantId, x.PlatformId })
			.HasDatabaseName("IX_PlatformAccount_Tenant_Platform");

		// Choose your uniqueness rule. This is a common MVP one:
		e.HasIndex(x => new { x.TenantId, x.AccountId, x.PlatformId })
			.IsUnique()
			.HasDatabaseName("UX_PlatformAccount_Tenant_Account_Platform");
	}

	private static void DefineAssetConfiguration(ModelBuilder modelBuilder)
	{
		var e = modelBuilder.Entity<Asset>();
		e.ToTable(DbConstants.Domain.EntityNames.AssetEntityName);

		e.HasKey(x => x.Id);

		e.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<AssetId>(x => x.Value, AssetId.From));

		// AssetCode is a VO: ensure you have converter
		e.Property(x => x.Code)
			.HasConversion(
				v => v.Value,
				v => AssetCode.Create(v)
			)
			.HasMaxLength(EntityConstraints.Domain.Asset.CodeMaxLength)
			.IsRequired();

		e.Property(x => x.Name)
			.HasMaxLength(EntityConstraints.Domain.Asset.NameMaxLength)
			.IsRequired();

		e.Property(x => x.NativeDecimals)
			.IsRequired();

		e.Property(x => x.Kind)
			.IsRequired()
			.HasConversion<int>();

		e.Property(x => x.State)
			.IsRequired()
			.HasConversion<int>();

		// RowVersion: optional but safe for catalog edits
		e.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		e.HasIndex(x => new {x.Code, x.Kind})
			.IsUnique()
			.HasDatabaseName("UX_Asset_Code_Kind");

		e.HasIndex(x => x.State)
			.HasDatabaseName("IX_Asset_State");
	}

	private static void DefineActivityConfiguration(ModelBuilder modelBuilder)
	{
		var e = modelBuilder.Entity<AssetActivityBase>();
		e.ToTable(DbConstants.Domain.EntityNames.ActivityEntityName);

		e.HasKey(x => x.Id);

		e.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<ActivityId>(x => x.Value, ActivityId.From));

		e.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		e.Property(x => x.PlatformAccountId)
			.HasConversion(new StrongIdConverter<PlatformAccountId>(x => x.Value, PlatformAccountId.From))
			.IsRequired();

		e.Property(x => x.Kind)
			.HasConversion<int>()
			.IsRequired();

		// Facts ordering: OccurredAt then Id, scoped by PlatformAccount
		e.HasIndex(x => new { x.PlatformAccountId, x.OccurredAt, x.Id })
			.HasDatabaseName("IX_Activity_PlatformAccount_OccurredAt_Id");

		// Discriminator: Kind
		e.HasDiscriminator(x => x.Kind)
			.HasValue<TradeActivity>(AssetActivityKind.Trade)
			.HasValue<BurnActivity>(AssetActivityKind.Burn)
			.HasValue<IncomeActivity>(AssetActivityKind.Income)
			.HasValue<ServiceActivity>(AssetActivityKind.Service)
			.HasValue<TransferActivity>(AssetActivityKind.Transfer)
			.HasValue<UserCorrectionActivity>(AssetActivityKind.UserCorrection)
			.HasValue<ExchangeActivity>(AssetActivityKind.Exchange);

		// ExternalMetadata moved from OwnsOne to a Complex Property
		e.ComplexProperty(x => x.ExternalMetadata, meta =>
		{
			meta.Property(m => m.ExternalId)
				.HasColumnName("ExternalId")
				.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

			meta.Property(m => m.Fingerprint)
				.HasColumnName("Fingerprint")
				.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

			meta.Property(m => m.Source)
				.HasColumnName("ExternalSource")
				.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

			meta.Property(m => m.Notes)
				.HasColumnName("ExternalNotes") 
				.HasMaxLength(EntityConstraints.CommonSettings.ExternalNotesMaxLength);
		});

		// Configure the derived Activities specific properties
		// trade
		modelBuilder.Entity<TradeActivity>(trade =>
		{
			trade.OwnsOne(x => x.Futures, fb =>
			{
				fb.Property(f => f.InstrumentKey)
					.HasColumnName("Futures_InstrumentKey")
					.HasMaxLength(EntityConstraints.Domain.Activity.FuturesInstrumentKeyLength);

				fb.Property(f => f.ContractKind)
					.HasColumnName("Futures_ContractKind")
					.HasConversion<int>();

				fb.Property(f => f.BaseAssetCode)
					.HasColumnName("Futures_BaseAssetCode")
					.HasMaxLength(EntityConstraints.Domain.Asset.CodeMaxLength);

				fb.Property(f => f.QuoteAssetCode)
					.HasColumnName("Futures_QuoteAssetCode")
					.HasMaxLength(EntityConstraints.Domain.Asset.CodeMaxLength);

				fb.Property(f => f.PositionEffect)
					.HasColumnName("Futures_PositionEffect")
					.HasConversion<int>();
			});

			// transfer
			modelBuilder.Entity<TransferActivity>(transfer =>
			{
				transfer.Property(x => x.Reference)
					.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

				transfer.Property(x => x.Counterparty)
					.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);
			});

		});

		// Idempotency uniqueness (filtered)
		// should be handled with migration file directly to preserve purity of entity, inside generated Up method

		// No RowVersion: Activity should be immutable (facts)
	}

	private static void DefineActivityLegsConfiguration(ModelBuilder modelBuilder)
	{
		var e = modelBuilder.Entity<AssetMovementLeg>();
		e.ToTable(DbConstants.Domain.EntityNames.ActivityLegEntityName);

		e.HasKey(x => x.Id);

		e.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<LegId>(x => x.Value, LegId.From));

		e.Property(x => x.ActivityId)
			.HasConversion(new StrongIdConverter<ActivityId>(x => x.Value, ActivityId.From))
			.IsRequired();

		e.Property(x => x.AssetId)
			.HasConversion(new StrongIdConverter<AssetId>(x => x.Value, AssetId.From))
			.IsRequired();

		e.Property(x => x.Direction)
			.IsRequired()
			.HasConversion<int>();

		e.Property(x => x.Role)
			.IsRequired()
			.HasConversion<int>();

		e.Property(x => x.Allocation)
			.IsRequired()
			.HasConversion<int>();

		// Quantity is VO: map to decimal
		e.Property(x => x.Amount)
			.HasConversion(
				v => v.Value,
				v => Quantity.Create(v)
			)
			.HasColumnType("decimal(38, 18)") 
			.IsRequired();

		e.Property(x => x.InstrumentKey)
			.HasMaxLength(EntityConstraints.Domain.ActivityLeg.InstrumentKeyMaxLength);

		e.HasIndex(x => x.ActivityId)
			.HasDatabaseName("IX_Leg_ActivityId");

		e.HasIndex(x => x.AssetId)
			.HasDatabaseName("IX_Leg_AssetId");

		// No RowVersion: as legs are immutable if activity is immutable
	}

	private static void DefineRestrictionJoinTables(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<TenantRestrictedAsset>(b =>
		{
			b.ToTable(DbConstants.Domain.EntityNames.TenantRestrictedAssetEntityName);
			b.HasKey(x => new { x.TenantId, x.AssetId });

			b.Property(x => x.TenantId)
				.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From));

			b.Property(x => x.AssetId)
				.HasConversion(new StrongIdConverter<AssetId>(x => x.Value, AssetId.From));

			b.HasIndex(x => x.TenantId).HasDatabaseName("IX_TenantRestrictedAsset_TenantId");
		});

		modelBuilder.Entity<TenantRestrictedPlatform>(b =>
		{
			b.ToTable(DbConstants.Domain.EntityNames.TenantRestrictedPlatformEntityName);
			b.HasKey(x => new { x.TenantId, x.PlatformId });

			b.Property(x => x.TenantId)
				.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From));

			b.Property(x => x.PlatformId)
				.HasConversion(new StrongIdConverter<PlatformId>(x => x.Value, PlatformId.From));

			b.HasIndex(x => x.TenantId).HasDatabaseName("IX_TenantRestrictedPlatform_TenantId");
		});
	}

	private static void ConfigureRelationships(ModelBuilder modelBuilder)
	{
		// Tenant -> Accounts
		modelBuilder.Entity<Account>()
			.HasOne<Tenant>()
			.WithMany()
			.HasForeignKey(a => a.TenantId)
			.OnDelete(DeleteBehavior.Restrict);

		// Account -> PlatformAccounts
		modelBuilder.Entity<PlatformAccount>()
			.HasOne<Account>()
			.WithMany()
			.HasForeignKey(pa => pa.AccountId)
			.OnDelete(DeleteBehavior.Restrict);

		// Platform -> PlatformAccounts
		modelBuilder.Entity<PlatformAccount>()
			.HasOne<Platform>()
			.WithMany()
			.HasForeignKey(pa => pa.PlatformId)
			.OnDelete(DeleteBehavior.Restrict);

		// Tenant -> PlatformAccounts
		modelBuilder.Entity<PlatformAccount>()
			.HasOne<Tenant>()
			.WithMany()
			.HasForeignKey(pa => pa.TenantId)
			.OnDelete(DeleteBehavior.Restrict);

		// PlatformAccount -> Activities
		modelBuilder.Entity<AssetActivityBase>()
			.HasOne<PlatformAccount>()
			.WithMany()
			.HasForeignKey(a => a.PlatformAccountId)
			.OnDelete(DeleteBehavior.Restrict);

		// Activity -> Legs
		modelBuilder.Entity<AssetMovementLeg>()
			.HasOne<AssetActivityBase>()
			.WithMany(a => a.Legs)
			.HasForeignKey(l => l.ActivityId)
			.OnDelete(DeleteBehavior.Cascade);

		// Optional: if legs reference Asset (recommended for integrity)
		modelBuilder.Entity<AssetMovementLeg>()
			.HasOne<Core.Assets.Models.Asset>()
			.WithMany()
			.HasForeignKey(l => l.AssetId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
