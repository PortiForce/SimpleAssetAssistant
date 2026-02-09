using Microsoft.EntityFrameworkCore;
using Portiforce.SAA.Core.Activities.Models.Activities;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Auth;
using Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Core;
using Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Ledger;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts;

public class AssetAssistantDbContext(DbContextOptions<AssetAssistantDbContext> options) : DbContext(options)
{
	// Core
	public DbSet<Tenant> Tenants => Set<Tenant>();
	public DbSet<Account> Accounts => Set<Account>();
	public DbSet<Platform> Platforms => Set<Platform>();
	public DbSet<PlatformAccount> PlatformAccounts => Set<PlatformAccount>();
	public DbSet<Asset> Assets => Set<Asset>();

	// Core-Join tables
	public DbSet<TenantRestrictedAsset> TenantRestrictedAssets => Set<TenantRestrictedAsset>();
	public DbSet<TenantRestrictedPlatform> TenantRestrictedPlatforms => Set<TenantRestrictedPlatform>();

	// Ledger
	public DbSet<AssetActivityBase> Activities => Set<AssetActivityBase>();
	public DbSet<AssetMovementLeg> ActivityLegs => Set<AssetMovementLeg>();

	// Auth
	public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();
	public DbSet<PasskeyCredential> PasskeyCredentials => Set<PasskeyCredential>();
	public DbSet<AuthSessionToken> AuthSessionTokens => Set<AuthSessionToken>();


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
		modelBuilder.HasDefaultSchema(DbConstants.Domain.Entities.DefaultSchemaName);

		// Core
		modelBuilder.ApplyConfiguration(new TenantConfiguration());
		modelBuilder.ApplyConfiguration(new PlatformConfiguration());
		modelBuilder.ApplyConfiguration(new AssetConfiguration());
		modelBuilder.ApplyConfiguration(new AccountConfiguration());
		modelBuilder.ApplyConfiguration(new PlatformAccountConfiguration());

		modelBuilder.ApplyConfiguration(new TenantRestrictedAssetConfiguration());
		modelBuilder.ApplyConfiguration(new TenantRestrictedPlatformConfiguration());

		// Ledger
		modelBuilder.ApplyConfiguration(new AssetActivityBaseConfiguration());
		modelBuilder.ApplyConfiguration(new TradeActivityConfiguration());
		modelBuilder.ApplyConfiguration(new TransferActivityConfiguration());
		modelBuilder.ApplyConfiguration(new AssetMovementLegConfiguration());

		// Auth
		modelBuilder.ApplyConfiguration(new ExternalIdentityConfiguration());
		modelBuilder.ApplyConfiguration(new PasskeyCredentialConfiguration());
		modelBuilder.ApplyConfiguration(new AuthSessionTokenConfiguration());

		// relationships
		ConfigureRelationships(modelBuilder);
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

		// External identity -> Account
		modelBuilder.Entity<ExternalIdentity>()
			.HasOne<Account>()
			.WithMany() // Account doesn't need a list of these
			.HasForeignKey(x => x.AccountId)
			.OnDelete(DeleteBehavior.Cascade); // If Account is deleted, remove the Provuder link

		modelBuilder.Entity<ExternalIdentity>()
			.HasOne<Tenant>()
			.WithMany() // Tenant doesn't need a list of these
			.HasForeignKey(x => x.TenantId)
			.OnDelete(DeleteBehavior.Cascade); // If Tenant is deleted, remove the Provider link

		// PassKeysCredentials -> Account
		modelBuilder.Entity<PasskeyCredential>()
			.HasOne<Account>()
			.WithMany()
			.HasForeignKey(x => x.AccountId)
			.OnDelete(DeleteBehavior.Cascade);  // If Account is deleted, remove the Passkey credentials

		// AuthSessionToken -> Account
		modelBuilder.Entity<AuthSessionToken>()
			.HasOne<Account>()
			.WithMany() // Account doesn't need a list of these
			.HasForeignKey(x => x.AccountId)
			.OnDelete(DeleteBehavior.Cascade); // If Account is deleted, remove the AuthSessionTokens link

		// AuthSessionToken->Tenant
		modelBuilder.Entity<AuthSessionToken>()
			.HasOne<Tenant>()
			.WithMany() // Tenant doesn't need a list of these
			.HasForeignKey(x => x.TenantId)
			.OnDelete(DeleteBehavior.Cascade); // If Tenant is deleted, remove the AuthSessionTokens link
	}
}
