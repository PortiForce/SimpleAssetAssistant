using Portiforce.SAA.Application.Enums;
using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Application.Models.Common.Messaging;

public sealed class InboxMessage : IDetailsProjection, IEntity<Guid>
{
	// EF Core constructor
	private InboxMessage()
	{
	}

	private InboxMessage(
		Guid id,
		TenantId tenantId,
		string publicReference,
		string type,
		string payloadJson,
		string source,
		string requestPath,
		string httpMethod,
		string idempotencyKey,
		DateTimeOffset receivedAtUtc,
		string? remoteIpAddress,
		string? userAgent)
	{
		if (id == Guid.Empty)
		{
			throw new ArgumentException("Inbox message id cannot be empty.", nameof(id));
		}

		if (tenantId.IsEmpty)
		{
			throw new ArgumentException("TenantId is not defined.", nameof(tenantId));
		}

		if (receivedAtUtc == default)
		{
			throw new ArgumentException("ReceivedAtUtc is not defined.", nameof(receivedAtUtc));
		}

		this.PublicReference = NormalizeRequired(
			publicReference,
			nameof(publicReference),
			EntityConstraints.Domain.InfrastructureMessage.PublicReferenceMaxLength);
		this.Type = NormalizeRequired(type, nameof(type), EntityConstraints.Domain.InfrastructureMessage.TypeMaxLength);
		this.PayloadJson = NormalizeRequired(payloadJson, nameof(payloadJson));
		this.Source = NormalizeRequired(source, nameof(source), EntityConstraints.Domain.InfrastructureMessage.SourceMaxLength);
		this.RequestPath = NormalizeRequired(
			requestPath,
			nameof(requestPath),
			EntityConstraints.Domain.InfrastructureMessage.RequestPathMaxLength);
		this.HttpMethod = NormalizeRequired(
			httpMethod,
			nameof(httpMethod),
			EntityConstraints.Domain.InfrastructureMessage.HttpMethodMaxLength).ToUpperInvariant();
		this.IdempotencyKey = NormalizeRequired(
			idempotencyKey,
			nameof(idempotencyKey),
			EntityConstraints.Domain.InfrastructureMessage.IdempotencyKeyMaxLength);
		this.RemoteIpAddress = NormalizeOptional(
			remoteIpAddress,
			EntityConstraints.Domain.InfrastructureMessage.RemoteIpAddressMaxLength);
		this.UserAgent = NormalizeOptional(userAgent, EntityConstraints.Domain.InfrastructureMessage.UserAgentMaxLength);

		this.Id = id;
		this.TenantId = tenantId;
		this.ReceivedAtUtc = receivedAtUtc;

		this.State = InboxMessageState.Received;
		this.AttemptCount = 0;
		this.ProcessingStartedAtUtc = null;
		this.ProcessedAtUtc = null;
		this.LastError = null;
		this.NextAttemptAtUtc = receivedAtUtc;
	}

	public TenantId TenantId { get; private set; }

	public string PublicReference { get; private set; } = default!;

	public string Type { get; private set; } = default!;

	public string PayloadJson { get; private set; } = default!;

	public string Source { get; private set; } = default!;

	public string RequestPath { get; private set; } = default!;

	public string HttpMethod { get; private set; } = default!;

	public string? RemoteIpAddress { get; private set; }

	public string? UserAgent { get; private set; }

	public InboxMessageState State { get; private set; }

	public DateTimeOffset ReceivedAtUtc { get; }

	public DateTimeOffset? ProcessingStartedAtUtc { get; private set; }

	public DateTimeOffset? ProcessedAtUtc { get; private set; }

	public int AttemptCount { get; private set; }

	public DateTimeOffset? NextAttemptAtUtc { get; private set; }

	public string? LastError { get; private set; }

	public string IdempotencyKey { get; private set; } = default!;

	public Guid Id { get; }

	public static InboxMessage Create(
		TenantId tenantId,
		string publicReference,
		string type,
		string payloadJson,
		string source,
		string requestPath,
		string httpMethod,
		string idempotencyKey,
		DateTimeOffset receivedAtUtc,
		string? remoteIpAddress = null,
		string? userAgent = null,
		Guid? id = null)
	{
		return new InboxMessage(
			id != null && id.Value != Guid.Empty
				? id.Value
			: GuidExtensions.New(),
			tenantId,
			publicReference,
			type,
			payloadJson,
			source,
			requestPath,
			httpMethod,
			idempotencyKey,
			receivedAtUtc,
			remoteIpAddress,
			userAgent);
	}

	public void MarkProcessing(DateTimeOffset processingStartedAtUtc)
	{
		this.EnforceDateValidations(processingStartedAtUtc, nameof(processingStartedAtUtc));

		if (this.State == InboxMessageState.Processed)
		{
			return;
		}

		if (this.State == InboxMessageState.Dead)
		{
			throw new InvalidOperationException("Dead inbox message cannot be marked as processing.");
		}

		this.State = InboxMessageState.Processing;
		this.ProcessingStartedAtUtc = processingStartedAtUtc;
		this.LastError = null;
	}

	public void MarkProcessed(DateTimeOffset processedAtUtc)
	{
		this.EnforceDateValidations(processedAtUtc, nameof(processedAtUtc));

		if (this.State == InboxMessageState.Processed)
		{
			return;
		}

		if (this.State == InboxMessageState.Dead)
		{
			throw new InvalidOperationException("Dead inbox message cannot be marked as processed.");
		}

		this.State = InboxMessageState.Processed;
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

		if (this.State == InboxMessageState.Processed)
		{
			return;
		}

		if (this.State == InboxMessageState.Dead)
		{
			return;
		}

		this.AttemptCount++;

		this.LastError = Truncate(
			NormalizeRequired(error, nameof(error)),
			EntityConstraints.Domain.InfrastructureMessage.LastErrorMaxLength);

		if (this.AttemptCount >= maxAttempts)
		{
			this.State = InboxMessageState.Dead;
			this.NextAttemptAtUtc = null;
			this.ProcessedAtUtc = null;
			return;
		}

		this.State = InboxMessageState.Failed;
		this.NextAttemptAtUtc = nextAttemptAtUtc;
	}

	public void ResetForRetry(DateTimeOffset nextAttemptAtUtc)
	{
		if (this.State is not InboxMessageState.Failed)
		{
			throw new InvalidOperationException("Only failed inbox messages can be reset for retry.");
		}

		this.NextAttemptAtUtc = nextAttemptAtUtc;
	}

	private void EnforceDateValidations(DateTimeOffset value, string parameterName)
	{
		if (value < this.ReceivedAtUtc)
		{
			throw new ArgumentException(
				$"{parameterName} cannot be before ReceivedAtUtc.",
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

	private static string? NormalizeOptional(string? value, int maxLength)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}

		string normalized = value.Trim();

		return Truncate(normalized, maxLength);
	}

	private static string Truncate(string value, int maxLength)
	{
		return value.Length <= maxLength
			? value
			: value[..maxLength];
	}
}
