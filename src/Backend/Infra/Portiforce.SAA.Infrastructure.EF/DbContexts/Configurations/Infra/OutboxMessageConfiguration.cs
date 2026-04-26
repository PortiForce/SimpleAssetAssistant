using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Infra;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
	public void Configure(EntityTypeBuilder<OutboxMessage> builder)
	{
		_ = builder.ToTable(
			DbConstants.Domain.Entities.InfrastructureSchema.OutboxMessageTableName,
			DbConstants.Domain.Entities.InfrastructureSchema.SchemaName);

		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever();

		_ = builder.Property(x => x.TenantId)
			.HasConversion(new StrongIdConverter<TenantId>(x => x.Value, TenantId.From))
			.IsRequired();

		// Concurrency (shadow)

		// Indexes
	}
}