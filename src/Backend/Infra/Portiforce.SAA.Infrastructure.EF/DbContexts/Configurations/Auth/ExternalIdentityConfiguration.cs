using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Auth;

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
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<ExternalIdentityId>(x => x.Value, ExternalIdentityId.From));

		// 3. Properties: 
		builder.Property(x => x.Provider)
			.IsRequired()
			.HasConversion<byte>();

		builder.Property(x => x.ProviderSubject)
			.IsRequired()
			.HasMaxLength(EntityConstraints.CommonSettings.ProviderSubjectMaxLength);

		// 4. Relationships
		builder.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		// 5. The Login Lookup Index
		builder.HasIndex(x => new { x.Provider, x.ProviderSubject })
			.IsUnique()
			.HasDatabaseName("UX_ExternalIdentity_Provider_ExternalId");
	}
}
