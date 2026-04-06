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
		_ = builder.ToTable(
			DbConstants.Domain.Entities.AuthSchema.ExternalIdentityTableName,
			DbConstants.Domain.Entities.AuthSchema.SchemaName);

		// 2. Primary Key
		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<ExternalIdentityId>(x => x.Value, ExternalIdentityId.From));

		// 3. Properties
		_ = builder.Property(x => x.Provider)
			.IsRequired()
			.HasConversion<byte>();

		_ = builder.Property(x => x.ProviderSubject)
			.IsRequired()
			.HasMaxLength(EntityConstraints.CommonSettings.ProviderSubjectMaxLength);

		// 4. Relationships
		_ = builder.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		_ = builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		// 5. The Login Lookup Index
		_ = builder.HasIndex(x => new { x.Provider, x.ProviderSubject })
			.IsUnique()
			.HasDatabaseName("UX_ExternalIdentity_Provider_ExternalId");
	}
}