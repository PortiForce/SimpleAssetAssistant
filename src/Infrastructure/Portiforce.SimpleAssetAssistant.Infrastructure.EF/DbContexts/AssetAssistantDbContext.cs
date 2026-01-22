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
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Extensions;

using Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

public class AssetAssistantDbContext(DbContextOptions<AssetAssistantDbContext> options) : DbContext(options)
{
	public DbSet<Tenant> Tenants => Set<Tenant>();
	public DbSet<Account> Accounts => Set<Account>();

	public DbSet<Platform> Platforms => Set<Platform>();
	public DbSet<PlatformAccount> PlatformAccounts => Set<PlatformAccount>();

	public DbSet<Core.Assets.Models.Asset> Assets => Set<Core.Assets.Models.Asset>();

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

		// adjust length to your Email VO constraints
		e.Property(x => x.Contact.Email)
			.HasMaxLength(EntityConstraints.Domain.Account.EmailMaxLength)
			.IsRequired();

		e.Property(x => x.Alias)
			.HasMaxLength(EntityConstraints.Domain.Account.AliasMaxLength)
			.IsRequired();

		// RowVersion on mutable aggregate root
		e.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		// Email unique within tenant
		e.HasIndex(x => new { x.TenantId, x.Contact.Email })
			.IsUnique()
			.HasDatabaseName("UX_Account_Tenant_Email");

		e.HasIndex(x => x.TenantId)
			.HasDatabaseName("IX_Account_TenantId");

		e.HasIndex(x => x.State)
			.HasDatabaseName("IX_Account_State");
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
		var e = modelBuilder.Entity<Core.Assets.Models.Asset>();
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

		// RowVersion: optional but safe for catalog edits
		e.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		e.HasIndex(x => x.Code)
			.IsUnique()
			.HasDatabaseName("UX_Asset_Code");

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

		// Facts ordering: OccurredAt then Id, scoped by PlatformAccount
		e.HasIndex(x => new { x.PlatformAccountId, x.OccurredAt, x.Id })
			.HasDatabaseName("IX_Activity_PlatformAccount_OccurredAt_Id");

		// Discriminator: Kind
		e.HasDiscriminator(x => x.Kind)
			.HasValue<TradeActivity>(AssetActivityKind.Trade)
			.HasValue<ExchangeActivity>(AssetActivityKind.Exchange);

		// ExternalMetadata as owned type with explicit column names (for stable filters/indexes)
		e.OwnsOne(x => x.ExternalMetadata, meta =>
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
		});

		// Idempotency uniqueness (filtered)
		e.HasIndex(x => new { x.TenantId, x.PlatformAccountId, x.Kind, x.ExternalMetadata.ExternalId })
			.IsUnique()
			.HasDatabaseName("UX_Activity_ExternalId")
			.HasFilter("[ExternalId] IS NOT NULL");

		e.HasIndex(x => new { x.TenantId, x.PlatformAccountId, x.Kind, x.ExternalMetadata.Fingerprint })
			.IsUnique()
			.HasDatabaseName("UX_Activity_Fingerprint")
			.HasFilter("[Fingerprint] IS NOT NULL");

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

		// Quantity is VO: map to decimal
		e.Property(x => x.Amount)
			.HasConversion(
				v => v.Value,
				v => Quantity.Create(v)
			)
			.HasColumnType("decimal(38, 18)") 
			.IsRequired();

		e.Property(x => x.Direction).IsRequired();
		e.Property(x => x.Role).IsRequired();
		e.Property(x => x.Allocation).IsRequired();

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

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		// setup common audit props (CreatedAt/UpdatedAt) if you have them
		return base.SaveChangesAsync(cancellationToken);
	}
}
