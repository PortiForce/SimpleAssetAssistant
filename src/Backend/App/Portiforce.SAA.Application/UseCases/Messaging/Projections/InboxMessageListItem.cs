using Portiforce.SAA.Application.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Messaging.Projections;

public sealed record InboxMessageListItem(
	Guid Id,
	TenantId TenantId,
	string PublicReference,
	string Type,
	string Source,
	string RequestPath,
	string HttpMethod,
	InboxMessageState State,
	DateTimeOffset ReceivedAtUtc,
	DateTimeOffset? NextAttemptAtUtc,
	int AttemptCount);
