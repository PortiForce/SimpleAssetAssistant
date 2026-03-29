using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Auth;

public sealed class PasskeyCredentialConfiguration : IEntityTypeConfiguration<PasskeyCredential>
{
	// todo tech: review
	public void Configure(EntityTypeBuilder<PasskeyCredential> builder)
	{
		// 1. Table Name
		_ = builder.ToTable(
			DbConstants.Domain.Entities.AuthSchema.PasskeyCredentialsTableName,
			DbConstants.Domain.Entities.AuthSchema.SchemaName);

		// 2. Primary Key
		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<ExternalIdentityId>(x => x.Value, ExternalIdentityId.From));

		// 3. Properties

		// The ID given by the browser/authenticator. Used for lookup during login
		_ = builder.Property(x => x.CredentialId)
			.IsRequired()
			.HasMaxLength(500); // Base64URL strings can be long

		// The public key verified against signatures
		_ = builder.Property(x => x.PublicKey)
			.IsRequired()
			.HasColumnType(DbConstants.CommonSettings.VarBinaryDataType);

		_ = builder.Property(x => x.SignatureCounter)
			.IsRequired();

		_ = builder.Property(x => x.UserHandle)
			.HasMaxLength(128);

		// 4. Relationships
		_ = builder.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		// 5. Indexes
		// Fast lookup during the "Login Finish" phase
		_ = builder.HasIndex(x => x.CredentialId)
			.IsUnique()
			.HasDatabaseName("UX_PasskeyCredential_CredentialId");
	}
}