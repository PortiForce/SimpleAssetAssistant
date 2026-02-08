using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Auth;

public sealed class AuthSessionTokenConfiguration : IEntityTypeConfiguration<AuthSessionToken>
{
	// todo tech: review
	public void Configure(EntityTypeBuilder<AuthSessionToken> builder)
	{
		// 1. Table Name
		builder.ToTable(
			DbConstants.Domain.Entities.AuthSchema.SessionTokenTableName,
			schema: DbConstants.Domain.Entities.AuthSchema.SchemaName);

		// 2. Primary Key
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedNever();

		// 3. Properties: 
		builder.Property(x => x.SessionId)
			.IsRequired();

		builder.Property(x => x.TokenHash)
			.IsRequired()
			.HasColumnType(DbConstants.CommonSettings.Varbinary32DataType);

		builder.Property(x => x.ReplacedByTokenHash)
			.HasColumnType(DbConstants.CommonSettings.Varbinary32DataType);

		builder.Property(x => x.CreatedAt).IsRequired();
		builder.Property(x => x.ExpiresAt).IsRequired();

		builder.Property(x => x.RevokedReason)
			.HasConversion<byte>();

		builder.Property(x => x.CreatedByIp)
			.HasMaxLength(EntityConstraints.Domain.AuthSessionToken.IpAddressMaxLength);

		builder.Property(x => x.RevokedByIp)
			.HasMaxLength(EntityConstraints.Domain.AuthSessionToken.IpAddressMaxLength);

		builder.Property(x => x.CreatedUserAgent)
			.HasMaxLength(EntityConstraints.Domain.AuthSessionToken.UserAgentMaxLength);

		builder.Property(x => x.UserAgentFingerprint)
			.HasMaxLength(EntityConstraints.Domain.AuthSessionToken.UserAgentFingerprint);

		// 4. Relationships
		builder.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		// 5. Indexes
		builder.HasIndex(x => x.TokenHash).IsUnique();
		builder.HasIndex(x => x.SessionId);
	}
}
