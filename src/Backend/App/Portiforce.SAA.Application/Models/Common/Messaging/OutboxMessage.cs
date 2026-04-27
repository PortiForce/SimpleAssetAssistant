using Portiforce.SAA.Application.Enums;
using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Application.Models.Common.Messaging;

public sealed class OutboxMessage : IDetailsProjection, IEntity<Guid>
{
	// EF Core constructor
	private OutboxMessage()
	{
	}

	private OutboxMessage(
		Guid id,
		TenantId tenantId,
		string type,
		string payloadJson,
		string idempotencyKey,
		DateTimeOffset createdAtUtc)
	{
		if (id == Guid.Empty)
		{
			throw new ArgumentException("Outbox message id cannot be empty.", nameof(id));
		}

		if (tenantId.IsEmpty)
		{
			throw new ArgumentException("TenantId is not defined.", nameof(tenantId));
		}

		if (createdAtUtc == default)
		{
			throw new ArgumentException("CreatedAtUtc is not defined.", nameof(createdAtUtc));
		}

		this.Type = NormalizeRequired(type, nameof(type), EntityConstraints.Domain.InfrastructureMessage.TypeMaxLength);
		this.PayloadJson = NormalizeRequired(payloadJson, nameof(payloadJson));
		this.IdempotencyKey = NormalizeRequired(
			idempotencyKey,
			nameof(idempotencyKey),
			EntityConstraints.Domain.InfrastructureMessage.IdempotencyKeyMaxLength);

		this.Id = id;
		this.TenantId = tenantId;
		this.CreatedAtUtc = createdAtUtc;

		this.State = OutboxMessageState.Pending;
		this.AttemptCount = 0;

		this.PublishedAtUtc = null;
		this.ProcessedAtUtc = null;
		this.LastError = null;
		this.NextAttemptAtUtc = createdAtUtc;
	}

	public TenantId TenantId { get; private set; }

	public string Type { get; private set; } = default!;

	public string PayloadJson { get; private set; } = default!;

	public OutboxMessageState State { get; private set; }

	public DateTimeOffset CreatedAtUtc { get; }

	public DateTimeOffset? PublishedAtUtc { get; private set; }

	public DateTimeOffset? ProcessedAtUtc { get; private set; }

	public int AttemptCount { get; private set; }

	public DateTimeOffset? NextAttemptAtUtc { get; private set; }

	public string? LastError { get; private set; }

	public string IdempotencyKey { get; private set; } = default!;

	public Guid Id { get; }

	public static OutboxMessage Create(
		TenantId tenantId,
		string type,
		string payloadJson,
		string idempotencyKey,
		DateTimeOffset createdAtUtc,
		Guid? id = null)
	{
		return new OutboxMessage(
			id != null && id.Value != Guid.Empty
				? id.Value
				: GuidExtensions.New(),
			tenantId,
			type,
			payloadJson,
			idempotencyKey,
			createdAtUtc);
	}

	public void MarkPublished(DateTimeOffset publishedAtUtc)
	{
		this.EnforceDateValidations(publishedAtUtc, nameof(publishedAtUtc));

		if (this.State == OutboxMessageState.Processed)
		{
			return;
		}

		if (this.State == OutboxMessageState.Dead)
		{
			throw new InvalidOperationException("Dead outbox message cannot be marked as published.");
		}

		this.State = OutboxMessageState.Published;
		this.PublishedAtUtc = publishedAtUtc;
		this.LastError = null;
	}

	public void MarkProcessed(DateTimeOffset processedAtUtc)
	{
		this.EnforceDateValidations(processedAtUtc, nameof(processedAtUtc));

		if (this.State == OutboxMessageState.Processed)
		{
			return;
		}

		if (this.State == OutboxMessageState.Dead)
		{
			throw new InvalidOperationException("Dead outbox message cannot be marked as processed.");
		}

		this.State = OutboxMessageState.Processed;
		this.ProcessedAtUtc = processedAtUtc;
		this.LastError = null;
		this.NextAttemptAtUtc = null;
	}

	public void MarkFailed(
		string error,
		DateTimeOffset failedAtUtc,
		DateTimeOffset nextAttemptAtUtc,
		int maxAttempts)
	{
		this.EnforceDateValidations(failedAtUtc, nameof(failedAtUtc));

		if (nextAttemptAtUtc <= failedAtUtc)
		{
			throw new ArgumentException(
				"Next attempt time must be after the failure time.",
				nameof(nextAttemptAtUtc));
		}

		if (maxAttempts <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(maxAttempts), "Max attempts must be positive.");
		}

		if (this.State == OutboxMessageState.Processed)
		{
			return;
		}

		if (this.State == OutboxMessageState.Dead)
		{
			return;
		}

		this.AttemptCount++;

		this.LastError = Truncate(
			NormalizeRequired(error, nameof(error)),
			EntityConstraints.Domain.InfrastructureMessage.LastErrorMaxLength);

		if (this.AttemptCount >= maxAttempts)
		{
			this.State = OutboxMessageState.Dead;
			this.NextAttemptAtUtc = null;
			this.ProcessedAtUtc = null;
			return;
		}

		this.State = OutboxMessageState.Failed;
		this.NextAttemptAtUtc = nextAttemptAtUtc;
	}

	public void ResetForRetry(DateTimeOffset nextAttemptAtUtc)
	{
		if (this.State is not OutboxMessageState.Failed)
		{
			throw new InvalidOperationException("Only failed outbox messages can be reset for retry.");
		}

		this.NextAttemptAtUtc = nextAttemptAtUtc;
	}

	private void EnforceDateValidations(DateTimeOffset value, string parameterName)
	{
		if (value < this.CreatedAtUtc)
		{
			throw new ArgumentException(
				$"{parameterName} cannot be before CreatedAtUtc.",
				parameterName);
		}
	}

	private static string NormalizeRequired(
		string value,
		string parameterName,
		int? maxLength = null)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException($"{parameterName} cannot be empty.", parameterName);
		}

		string normalized = value.Trim();

		if (maxLength is not null && normalized.Length > maxLength.Value)
		{
			throw new ArgumentException(
				$"{parameterName} cannot exceed {maxLength.Value} characters.",
				parameterName);
		}

		return normalized;
	}

	private static string Truncate(string value, int maxLength)
	{
		return value.Length <= maxLength
			? value
			: value[..maxLength];
	}
}