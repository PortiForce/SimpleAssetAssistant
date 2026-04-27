using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
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

		_ = builder.Property(x => x.Type)
			.HasMaxLength(EntityConstraints.Domain.InfrastructureMessage.TypeMaxLength)
			.IsRequired();

		_ = builder.Property(x => x.PayloadJson)
			.IsRequired();

		_ = builder.Property(x => x.State)
			.HasConversion<byte>()
			.IsRequired();

		_ = builder.Property(x => x.CreatedAtUtc)
			.IsRequired();

		_ = builder.Property(x => x.PublishedAtUtc);

		_ = builder.Property(x => x.ProcessedAtUtc);

		_ = builder.Property(x => x.AttemptCount)
			.IsRequired();

		_ = builder.Property(x => x.NextAttemptAtUtc);

		_ = builder.Property(x => x.LastError)
			.HasMaxLength(EntityConstraints.Domain.InfrastructureMessage.LastErrorMaxLength);

		_ = builder.Property(x => x.IdempotencyKey)
			.HasMaxLength(EntityConstraints.Domain.InfrastructureMessage.IdempotencyKeyMaxLength)
			.IsRequired();

		// Concurrency (shadow)
		_ = builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		// Indexes
		_ = builder.HasIndex(x => new { x.State, x.NextAttemptAtUtc, x.Type })
			.HasDatabaseName("IX_Outbox_State_NextAttempt_Type");

		_ = builder.HasIndex(x => x.TenantId)
			.HasDatabaseName("IX_Outbox_TenantId");

		_ = builder.HasIndex(x => x.IdempotencyKey)
			.IsUnique()
			.HasDatabaseName("UX_Outbox_IdempotencyKey");
	}
}