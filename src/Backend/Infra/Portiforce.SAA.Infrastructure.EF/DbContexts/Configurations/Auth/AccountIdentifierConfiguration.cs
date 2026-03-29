using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Auth;

public sealed class AccountIdentifierConfiguration : IEntityTypeConfiguration<AccountIdentifier>
{
	// todo tech: review
	public void Configure(EntityTypeBuilder<AccountIdentifier> builder)
	{
		// 1. Table Name
		_ = builder.ToTable(
			DbConstants.Domain.Entities.AuthSchema.AccountIdentifierTableName,
			DbConstants.Domain.Entities.AuthSchema.SchemaName);

		// 2. Primary Key
		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<AccountIdentifierId>(x => x.Value, AccountIdentifierId.From));

		// 3. Properties
		_ = builder.Property(x => x.Kind)
			.IsRequired()
			.HasConversion<byte>();

		_ = builder.Property(x => x.Value)
			.IsRequired()
			.HasMaxLength(EntityConstraints.CommonSettings.ProviderSubjectMaxLength);

		_ = builder.Property(x => x.IsVerified)
			.IsRequired()
			.HasConversion<bool>();

		_ = builder.Property(x => x.IsPrimary)
			.IsRequired()
			.HasConversion<bool>();

		// 4. Relationships
		_ = builder.Property(x => x.AccountId)
			.HasConversion(new StrongIdConverter<AccountId>(x => x.Value, AccountId.From))
			.IsRequired();

		_ = builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		// 5. The Login Lookup Index
		_ = builder.HasIndex(x => new { x.TenantId, x.Kind, x.Value })
			.IsUnique()
			.HasDatabaseName("UX_AccountIdentifier_Tenant_Kind_Value");
	}
}