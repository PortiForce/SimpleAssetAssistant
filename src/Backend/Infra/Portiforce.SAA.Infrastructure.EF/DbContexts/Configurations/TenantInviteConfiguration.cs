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
		_ = builder.ToTable(
			DbConstants.Domain.Entities.DefaultSchema.InviteTableName,
			DbConstants.Domain.Entities.DefaultSchema.SchemaName);

		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever();

		_ = builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		_ = builder.Property(x => x.InvitedByAccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		_ = builder.Property(x => x.AcceptedAccountId)
			.HasConversion(new NullableStrongIdConverter<AccountId>(x => x.Value, AccountId.From));

		_ = builder.Property(x => x.RevokedByAccountId)
			.HasConversion(new NullableStrongIdConverter<AccountId>(x => x.Value, AccountId.From));

		_ = builder.Property(x => x.BlockFutureInvites)
			.HasConversion<bool>()
			.IsRequired(false);

		// Owned InviteTarget
		_ = builder.ComplexProperty(
			x => x.InviteTarget,
			cb =>
			{
				// IMPORTANT: use explicit column names to keep indexes simple & stable
				_ = cb.Property(x => x.Value)
					.HasColumnName("InviteTargetValue")
					.IsRequired()
					.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

				_ = cb.Property(x => x.Kind)
					.HasColumnName("InviteTargetKind")
					.IsRequired()
					.HasConversion<byte>();

				_ = cb.Property(x => x.Channel)
					.HasColumnName("InviteTargetChannel")
					.IsRequired()
					.HasConversion<byte>();
			});

		_ = builder.Property(x => x.IntendedRole)
			.HasConversion<int>()
			.IsRequired();

		_ = builder.Property(x => x.IntendedTier)
			.HasConversion<int>()
			.IsRequired();

		_ = builder.Property(x => x.State)
			.HasConversion<int>()
			.IsRequired();

		_ = builder.Property(x => x.TokenHash)
			.HasMaxLength(32)
			.IsFixedLength()
			.IsRequired()
			.HasColumnType(DbConstants.CommonSettings.Varbinary32DataType);

		_ = builder.Property(x => x.CreatedAtUtc)
			.IsRequired();

		_ = builder.Property(x => x.ExpiresAtUtc)
			.IsRequired();

		_ = builder.Property(x => x.SentAtUtc);

		_ = builder.Property(x => x.UpdatedAtUtc);

		_ = builder.Property(x => x.SendCount)
			.IsRequired();

		// Concurrency (shadow)
		_ = builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		// Indexes
		_ = builder.HasIndex(x => x.TenantId)
			.HasDatabaseName("IX_Invite_TenantId");

		_ = builder.HasIndex(x => x.State)
			.HasDatabaseName("IX_Invite_State");

		// Optional: if you often query "active invites"
		_ = builder.HasIndex(x => new { x.TenantId, x.State, x.ExpiresAtUtc })
			.HasDatabaseName("IX_Invite_TenantId_State_ExpiresAt");

		// Invite target model is unique within tenant
		// should be handled with migration file directly to preserve purity of entity, inside generated Up method
	}
}