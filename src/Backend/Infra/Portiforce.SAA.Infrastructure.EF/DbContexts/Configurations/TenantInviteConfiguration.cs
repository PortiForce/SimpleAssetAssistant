using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations;

public sealed class TenantInviteConfiguration : IEntityTypeConfiguration<TenantInvite>
{
	public void Configure(EntityTypeBuilder<TenantInvite> builder)
	{
		builder.ToTable(
			DbConstants.Domain.Entities.DefaultSchema.InviteTableName,
			schema: DbConstants.Domain.Entities.DefaultSchema.SchemaName);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedNever();

		builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		builder.Property(x => x.InvitedByAccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		builder.Property(x => x.AcceptedAccountId)
			.HasConversion(new NullableStrongIdConverter<AccountId>(x => x.Value, AccountId.From));

		builder.Property(x => x.RevokedByAccountId)
			.HasConversion(new NullableStrongIdConverter<AccountId>(x => x.Value, AccountId.From));

		// Owned InviteTarget
		builder.ComplexProperty(x => x.InviteTarget, cb =>
		{
			// IMPORTANT: use explicit column names to keep indexes simple & stable
			cb.Property(x => x.Value)
				.HasColumnName("InviteTargetValue")
				.IsRequired()
				.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

			cb.Property(x => x.Type)
				.HasColumnName("InviteTargetType")
				.IsRequired()
				.HasConversion<int>();
		});

		builder.Property(x => x.IntendedRole)
			.HasConversion<int>()
			.IsRequired();

		builder.Property(x => x.IntendedTier)
			.HasConversion<int>()
			.IsRequired();

		builder.Property(x => x.State)
			.HasConversion<int>()
			.IsRequired();

		builder.Property(x => x.TokenHash)
			.HasMaxLength(32)
			.IsFixedLength()
			.IsRequired()
			.HasColumnType(DbConstants.CommonSettings.Varbinary32DataType);

		builder.Property(x => x.CreatedAtUtc)
			.IsRequired();

		builder.Property(x => x.ExpiresAtUtc)
			.IsRequired();

		builder.Property(x => x.SentAtUtc);

		builder.Property(x => x.AcceptedAtUtc);

		builder.Property(x => x.RevokedAtUtc);

		builder.Property(x => x.SendCount)
			.IsRequired();

		// Concurrency (shadow)
		builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		// Indexes
		builder.HasIndex(x => x.TenantId)
			.HasDatabaseName("IX_Invite_TenantId");

		builder.HasIndex(x => x.State)
			.HasDatabaseName("IX_Invite_State");

		// Optional: if you often query "active invites"
		builder.HasIndex(x => new { x.TenantId, x.State, x.ExpiresAtUtc })
			.HasDatabaseName("IX_Invite_TenantId_State_ExpiresAt");

		// Invite target model is unique within tenant
		// should be handled with migration file directly to preserve purity of entity, inside generated Up method
	}
}

