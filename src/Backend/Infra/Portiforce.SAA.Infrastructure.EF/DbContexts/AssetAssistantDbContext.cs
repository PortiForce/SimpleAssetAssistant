using Microsoft.EntityFrameworkCore;

using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Identity.Models.Client;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations;
using Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Auth;
using Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Core;
using Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Infra;
using Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Ledger;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts;

public class AssetAssistantDbContext(DbContextOptions<AssetAssistantDbContext> options) : DbContext(options)
{
	// Core
	public DbSet<Tenant> Tenants => this.Set<Tenant>();

	public DbSet<Account> Accounts => this.Set<Account>();

	public DbSet<Platform> Platforms => this.Set<Platform>();

	public DbSet<PlatformAccount> PlatformAccounts => this.Set<PlatformAccount>();

	public DbSet<Asset> Assets => this.Set<Asset>();

	// Core-Join tables
	public DbSet<TenantRestrictedAsset> TenantRestrictedAssets => this.Set<TenantRestrictedAsset>();

	public DbSet<TenantRestrictedPlatform> TenantRestrictedPlatforms => this.Set<TenantRestrictedPlatform>();

	// Ledger
	public DbSet<AssetActivityBase> Activities => this.Set<AssetActivityBase>();

	public DbSet<AssetMovementLeg> ActivityLegs => this.Set<AssetMovementLeg>();

	// Auth
	public DbSet<AccountIdentifier> AccountIdentifiers => this.Set<AccountIdentifier>();

	public DbSet<ExternalIdentity> ExternalIdentities => this.Set<ExternalIdentity>();

	public DbSet<PasskeyCredential> PasskeyCredentials => this.Set<PasskeyCredential>();

	public DbSet<AuthSessionToken> AuthSessionTokens => this.Set<AuthSessionToken>();

	// Functionality
	public DbSet<TenantInvite> Invites => this.Set<TenantInvite>();

	// Infra
	public DbSet<OutboxMessage> OutboxMessages => this.Set<OutboxMessage>();

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
		_ = modelBuilder.HasDefaultSchema(DbConstants.Domain.Entities.DefaultSchema.SchemaName);

		// Core
		_ = modelBuilder.ApplyConfiguration(new TenantConfiguration());
		_ = modelBuilder.ApplyConfiguration(new PlatformConfiguration());
		_ = modelBuilder.ApplyConfiguration(new AssetConfiguration());
		_ = modelBuilder.ApplyConfiguration(new AccountConfiguration());
		_ = modelBuilder.ApplyConfiguration(new PlatformAccountConfiguration());

		_ = modelBuilder.ApplyConfiguration(new TenantRestrictedAssetConfiguration());
		_ = modelBuilder.ApplyConfiguration(new TenantRestrictedPlatformConfiguration());

		// Ledger
		_ = modelBuilder.ApplyConfiguration(new AssetActivityBaseConfiguration());
		_ = modelBuilder.ApplyConfiguration(new TradeActivityConfiguration());
		_ = modelBuilder.ApplyConfiguration(new TransferActivityConfiguration());
		_ = modelBuilder.ApplyConfiguration(new AssetMovementLegConfiguration());

		// Auth
		_ = modelBuilder.ApplyConfiguration(new AccountIdentifierConfiguration());
		_ = modelBuilder.ApplyConfiguration(new ExternalIdentityConfiguration());
		_ = modelBuilder.ApplyConfiguration(new PasskeyCredentialConfiguration());
		_ = modelBuilder.ApplyConfiguration(new AuthSessionTokenConfiguration());

		// flow
		_ = modelBuilder.ApplyConfiguration(new TenantInviteConfiguration());

		// infra
		_ = modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

		// relationships
		ConfigureRelationships(modelBuilder);
	}

	private static void ConfigureRelationships(ModelBuilder modelBuilder)
	{
		// Tenant -> Accounts
		_ = modelBuilder.Entity<Account>()
			.HasOne<Tenant>()
			.WithMany()
			.HasForeignKey(a => a.TenantId)
			.OnDelete(DeleteBehavior.Restrict);

		// Account -> PlatformAccounts
		_ = modelBuilder.Entity<PlatformAccount>()
			.HasOne<Account>()
			.WithMany()
			.HasForeignKey(pa => pa.AccountId)
			.OnDelete(DeleteBehavior.Restrict);

		// Account -> AccountIdentifiers
		_ = modelBuilder.Entity<AccountIdentifier>()
			.HasOne<Account>()
			.WithMany() // Account doesn't need a list of these
			.HasForeignKey(x => x.AccountId)
			.OnDelete(DeleteBehavior.Cascade); // If Account is deleted, remove reserved identifiers

		// Platform -> PlatformAccounts
		_ = modelBuilder.Entity<PlatformAccount>()
			.HasOne<Platform>()
			.WithMany()
			.HasForeignKey(pa => pa.PlatformId)
			.OnDelete(DeleteBehavior.Restrict);

		// Tenant -> PlatformAccounts
		_ = modelBuilder.Entity<PlatformAccount>()
			.HasOne<Tenant>()
			.WithMany()
			.HasForeignKey(pa => pa.TenantId)
			.OnDelete(DeleteBehavior.Restrict);

		// PlatformAccount -> Activities
		_ = modelBuilder.Entity<AssetActivityBase>()
			.HasOne<PlatformAccount>()
			.WithMany()
			.HasForeignKey(a => a.PlatformAccountId)
			.OnDelete(DeleteBehavior.Restrict);

		// Activity -> Legs
		_ = modelBuilder.Entity<AssetMovementLeg>()
			.HasOne<AssetActivityBase>()
			.WithMany(a => a.Legs)
			.HasForeignKey(l => l.ActivityId)
			.OnDelete(DeleteBehavior.Cascade);

		// Optional: if legs reference Asset (recommended for integrity)
		_ = modelBuilder.Entity<AssetMovementLeg>()
			.HasOne<Asset>()
			.WithMany()
			.HasForeignKey(l => l.AssetId)
			.OnDelete(DeleteBehavior.Restrict);

		// External identity -> Account
		_ = modelBuilder.Entity<ExternalIdentity>()
			.HasOne<Account>()
			.WithMany() // Account doesn't need a list of these
			.HasForeignKey(x => x.AccountId)
			.OnDelete(DeleteBehavior.Cascade); // If Account is deleted, remove the Provider link

		_ = modelBuilder.Entity<ExternalIdentity>()
			.HasOne<Tenant>()
			.WithMany() // Tenant doesn't need a list of these
			.HasForeignKey(x => x.TenantId)
			.OnDelete(DeleteBehavior.Cascade); // If Tenant is deleted, remove the Provider link

		// PassKeysCredentials -> Account
		_ = modelBuilder.Entity<PasskeyCredential>()
			.HasOne<Account>()
			.WithMany()
			.HasForeignKey(x => x.AccountId)
			.OnDelete(DeleteBehavior.Cascade); // If Account is deleted, remove the Passkey credentials

		// AuthSessionToken -> Account
		_ = modelBuilder.Entity<AuthSessionToken>()
			.HasOne<Account>()
			.WithMany() // Account doesn't need a list of these
			.HasForeignKey(x => x.AccountId)
			.OnDelete(DeleteBehavior.Cascade); // If Account is deleted, remove the AuthSessionTokens link

		// AuthSessionToken->Tenant
		_ = modelBuilder.Entity<AuthSessionToken>()
			.HasOne<Tenant>()
			.WithMany() // Tenant doesn't need a list of these
			.HasForeignKey(x => x.TenantId)
			.OnDelete(DeleteBehavior.Cascade); // If Tenant is deleted, remove the AuthSessionTokens link

		// Tenant -> Invites
		_ = modelBuilder.Entity<TenantInvite>()
			.HasOne<Tenant>()
			.WithMany()
			.HasForeignKey(pa => pa.TenantId)
			.OnDelete(DeleteBehavior.Restrict);

		// Tenant -> AccountIdentifiers
		_ = modelBuilder.Entity<AccountIdentifier>()
			.HasOne<Tenant>()
			.WithMany() // Tenant doesn't need a list of these
			.HasForeignKey(x => x.TenantId)
			.OnDelete(DeleteBehavior.Cascade); // If Tenant is deleted, remove reserved identifiers

		// Account (who sent invite) -> Invite (same account can send many invites)
		_ = modelBuilder.Entity<TenantInvite>()
			.HasOne<Account>()
			.WithMany()
			.HasForeignKey(x => x.InvitedByAccountId)
			.OnDelete(DeleteBehavior.Restrict);

		// Account (who accepted invite) -> Invite
		_ = modelBuilder.Entity<TenantInvite>()
			.HasOne<Account>()
			.WithMany()
			.HasForeignKey(x => x.AcceptedAccountId)
			.OnDelete(DeleteBehavior.Restrict);

		// Tenant -> Outbox messages
		_ = modelBuilder.Entity<OutboxMessage>()
			.HasOne<Tenant>()
			.WithMany()
			.HasForeignKey(x => x.TenantId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
