using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Auth;

public sealed class AuthSessionTokenConfiguration : IEntityTypeConfiguration<AuthSessionToken>
{
	// todo tech: review
	public void Configure(EntityTypeBuilder<AuthSessionToken> builder)
	{
		// 1. Table Name
		_ = builder.ToTable(
			DbConstants.Domain.Entities.AuthSchema.SessionTokenTableName,
			DbConstants.Domain.Entities.AuthSchema.SchemaName);

		// 2. Primary Key
		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever();

		// 3. Properties: 
		_ = builder.Property(x => x.SessionId)
			.IsRequired();

		_ = builder.Property(x => x.TokenHash)
			.HasMaxLength(32)
			.IsFixedLength()
			.IsRequired()
			.HasColumnType(DbConstants.CommonSettings.Varbinary32DataType);

		_ = builder.Property(x => x.ReplacedByTokenHash)
			.HasColumnType(DbConstants.CommonSettings.Varbinary32DataType);

		_ = builder.Property(x => x.CreatedAt).IsRequired();
		_ = builder.Property(x => x.ExpiresAt).IsRequired();

		_ = builder.Property(x => x.RevokedReason)
			.HasConversion<byte>();

		_ = builder.Property(x => x.CreatedByIp)
			.HasMaxLength(EntityConstraints.Domain.AuthSessionToken.IpAddressMaxLength);

		_ = builder.Property(x => x.RevokedByIp)
			.HasMaxLength(EntityConstraints.Domain.AuthSessionToken.IpAddressMaxLength);

		_ = builder.Property(x => x.CreatedUserAgent)
			.HasMaxLength(EntityConstraints.Domain.AuthSessionToken.UserAgentMaxLength);

		_ = builder.Property(x => x.UserAgentFingerprint)
			.HasMaxLength(EntityConstraints.Domain.AuthSessionToken.UserAgentFingerprint);

		// 4. Relationships
		_ = builder.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		_ = builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		// 5. Indexes
		_ = builder.HasIndex(x => x.TokenHash).IsUnique();
		_ = builder.HasIndex(x => x.SessionId);
	}
}