using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;
using Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Auth;

public sealed class ExternalIdentityConfiguration : IEntityTypeConfiguration<ExternalIdentity>
{
	// todo tech: review
	public void Configure(EntityTypeBuilder<ExternalIdentity> builder)
	{
		// 1. Table Name
		builder.ToTable(
			DbConstants.Domain.Entities.AuthSchema.ExternalIdentityTableName,
			schema: DbConstants.Domain.Entities.AuthSchema.SchemaName);

		// 2. Primary Key
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id)
			.ValueGeneratedNever();

		// 3. Properties: 
		builder.Property(x => x.Provider)
			.IsRequired()
			.HasMaxLength(EntityConstraints.CommonSettings.ExternalIdMaxLength);

		// todo tech: used to be ExternalId
		builder.Property(x => x.ProviderSubject)
			.IsRequired()
			.HasMaxLength(EntityConstraints.CommonSettings.ProviderSubjectMaxLength);

		// 4. Relationships
		builder.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		// todo tech: used to be ExternalId
		// 5. The Login Lookup Index
		builder.HasIndex(x => new { x.Provider, x.ProviderSubject })
			.IsUnique()
			.HasDatabaseName("UX_ExternalIdentity_Provider_ExternalId");
	}
}
