using Portiforce.SAA.Application.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Messaging.Projections;

public sealed record InboxMessageDetails(
	Guid Id,
	TenantId TenantId,
	string PublicReference,
	string Type,
	string PayloadJson,
	string Source,
	string RequestPath,
	string HttpMethod,
	string? RemoteIpAddress,
	string? UserAgent,
	InboxMessageState State,
	DateTimeOffset ReceivedAtUtc,
	DateTimeOffset? ProcessingStartedAtUtc,
	DateTimeOffset? ProcessedAtUtc,
	int AttemptCount,
	DateTimeOffset? NextAttemptAtUtc,
	string? LastError,
	string IdempotencyKey);
