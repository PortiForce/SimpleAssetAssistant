using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;

using Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Auth;

public sealed class PasskeyCredentialConfiguration : IEntityTypeConfiguration<PasskeyCredential>
{
	// todo tech: review
	public void Configure(EntityTypeBuilder<PasskeyCredential> builder)
	{
		// 1. Table Name
		builder.ToTable(
			DbConstants.Domain.Entities.AuthSchema.PasskeyCredentialsTableName,
			schema: DbConstants.Domain.Entities.AuthSchema.SchemaName);

		// 2. Primary Key
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id)
			.ValueGeneratedNever();

		// 3. Properties

		// The ID given by the browser/authenticator. Used for lookup during login
		builder.Property(x => x.CredentialId)
			.IsRequired()
			.HasMaxLength(500); // Base64URL strings can be long

		// The public key verified against signatures
		builder.Property(x => x.PublicKey)
			.IsRequired()
			.HasColumnType(DbConstants.CommonSettings.VarBinaryDataType);

		builder.Property(x => x.SignatureCounter)
			.IsRequired();

		builder.Property(x => x.UserHandle)
			.HasMaxLength(128);

		// 4. Relationships
		builder.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		// 5. Indexes
		// Fast lookup during the "Login Finish" phase
		builder.HasIndex(x => x.CredentialId)
			.IsUnique()
			.HasDatabaseName("UX_PasskeyCredential_CredentialId");
	}
}
