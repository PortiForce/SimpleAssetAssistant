using Microsoft.EntityFrameworkCore;

using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Assets.Models;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Extensions;

using Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;

public class AssetAssistantDbContext(DbContextOptions<AssetAssistantDbContext> options) : DbContext(options)
{
	public DbSet<Tenant> Tenants { get; set; }
	public DbSet<Account> Accounts { get; set; }

	public DbSet<Platform> Platforms { get; set; }
	public DbSet<PlatformAccount> PlatformAccounts { get; set; }

	public DbSet<Asset> Assets { get; set; }

	public DbSet<AssetActivityBase> Activities { get; set; }

	public DbSet<AssetMovementLeg> ActivityLegs { get; set; }

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
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssetAssistantDbContext).Assembly);

		modelBuilder.HasDefaultSchema(DbConstants.Domain.DefaultSchemaName);

		// common configurations
		modelBuilder.ConfigureRowVersion();

		DefineTenantConfiguration(modelBuilder);

		DefineAccountConfiguration(modelBuilder);

		DefineAssetConfiguration(modelBuilder);

		DefineActivityConfiguration(modelBuilder);

		DefineActivityLegsConfiguration(modelBuilder);

		ConfigureRelationships(modelBuilder);
	}

	private void DefineActivityLegsConfiguration(ModelBuilder modelBuilder)
	{
		var e = modelBuilder.Entity<AssetMovementLeg>();
		e.ToTable(DbConstants.Domain.EntityNames.ActivityLegEntityName);

		e.HasKey(x => x.Id);
		e.Property(x => x.Id).ValueGeneratedNever();

		// custom converter value
		e.Property(x => x.Id)
			.HasConversion(new StrongIdConverter<LegId>(x => x.Value, LegId.From));

		e.HasIndex(x => x.ActivityId).HasDatabaseName("IX_Leg_ActivityId");

		// Strongly consider storing a Sequence/Ordinal so UI can reproduce order deterministically
		// e.Property(x => x.Ordinal).IsRequired();
	}

	private void DefineActivityConfiguration(ModelBuilder modelBuilder)
	{
		var e = modelBuilder.Entity<AssetActivityBase>();
		e.ToTable(DbConstants.Domain.EntityNames.ActivityEntityName);

		e.HasKey(x => x.Id);
		e.Property(x => x.Id).ValueGeneratedNever();

		e.Property(x => x.Id)
			.HasConversion(new StrongIdConverter<ActivityId>(x => x.Value, ActivityId.From));

		// Indexing for “facts ordered by OccurredAt then Id”
		e.HasIndex(x => new { x.PlatformAccountId, x.OccurredAt, x.Id })
			.HasDatabaseName("IX_Activity_PlatformAccount_OccurredAt_Id");

		// Discriminator
		e.HasDiscriminator(x => x.Kind)
			.HasValue<TradeActivity>(AssetActivityKind.Trade)
			.HasValue<ExchangeActivity>(AssetActivityKind.Exchange)
			.HasValue<TransferActivity>(AssetActivityKind.Transfer)
			.HasValue<TransferActivity>(AssetActivityKind.Burn)
			.HasValue<TransferActivity>(AssetActivityKind.Income)
			.HasValue<TransferActivity>(AssetActivityKind.Service)
			.HasValue<TransferActivity>(AssetActivityKind.UserCorrection);

		// to store ExternalMetadata, mapping as owned type
		e.OwnsOne(x => x.ExternalMetadata, meta => { });

		// Unique constraint for idempotency:
		// Either (TenantId, PlatformAccountId, Kind, ExternalId) OR (Fingerprint)

		e.HasIndex(x => new { x.TenantId, x.PlatformAccountId, x.Kind, x.ExternalMetadata.ExternalId })
			.IsUnique()
			.HasDatabaseName("UX_Activity_ExternalId")
			.HasFilter("[ExternalId] IS NOT NULL");

		e.HasIndex(x => new { x.TenantId, x.PlatformAccountId, x.Kind, x.ExternalMetadata.Fingerprint })
			.IsUnique()
			.HasDatabaseName("UX_Activity_Fingerprint")
			.HasFilter("[Fingerprint] IS NOT NULL");
	}

	private void DefineAssetConfiguration(ModelBuilder modelBuilder)
	{
		throw new NotImplementedException();
	}

	private void DefineAccountConfiguration(ModelBuilder modelBuilder)
	{
		throw new NotImplementedException();
	}

	private void DefineTenantConfiguration(ModelBuilder modelBuilder)
	{
		var e = modelBuilder.Entity<Tenant>();
		e.ToTable(DbConstants.Domain.EntityNames.TenantEntityName);

		e.HasKey(x => x.Id);

		// custom converter value
		e.Property(x => x.Id)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From));

		// as Id is Guid/Ulid value object: use ValueConverter
		e.Property(x => x.Id).ValueGeneratedNever();

		e.Property(x => x.Name)
			.HasMaxLength(EntityConstraints.Domain.Tenant.MaxNameLength)
			.IsRequired();

		e.Property(x => x.Code)
			.HasMaxLength(EntityConstraints.Domain.Tenant.MaxCodeLength)
			.IsRequired();

		e.Property(x => x.BrandName)
			.HasMaxLength(EntityConstraints.Domain.Tenant.MaxBrandLength);

		e.Property(x => x.DomainPrefix)
			.HasMaxLength(EntityConstraints.Domain.Tenant.MaxPublicDomainLength);

		// restricted assets
		e.HasMany<TenantRestrictedAsset>()
			.WithOne()
			.HasForeignKey(x => x.TenantId)
			.OnDelete(DeleteBehavior.Cascade);

		e.Navigation(x => x.RestrictedAssets).AutoInclude(false);

		// Indexes
		e.HasIndex(x => x.State)
			.HasDatabaseName("IX_Tenant_State");

		e.HasIndex(x => x.Code)
			.IsUnique()
			.HasDatabaseName("UX_Tenant_Code");

		// enforce domain prefix uniqueness only when value exists (SQL Server filter)
		e.HasIndex(x => x.DomainPrefix)
			.IsUnique()
			.HasDatabaseName("UX_Tenant_DomainPrefix")
			.HasFilter("[DomainPrefix] IS NOT NULL");

		modelBuilder.Entity<TenantRestrictedAsset>(b =>
		{
			b.ToTable("TenantRestrictedAssets");
			b.HasKey(x => new { x.TenantId, x.AssetId });
		});
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
	{
		// setup common Log properties (if needed)
		return base.SaveChangesAsync(cancellationToken);
	}

	private void ConfigureRelationships(ModelBuilder modelBuilder)
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
	}
}
