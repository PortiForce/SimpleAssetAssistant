using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Application.UseCases.Messaging.Projections;

namespace Portiforce.SAA.Application.UseCases.Messaging.Flow.Mappers;

public static class InboxMessageProjectionMapper
{
	public static InboxMessageListItem ToListItem(InboxMessage message) =>
		new(
			message.Id,
			message.TenantId,
			message.PublicReference,
			message.Type,
			message.Source,
			message.RequestPath,
			message.HttpMethod,
			message.State,
			message.ReceivedAtUtc,
			message.NextAttemptAtUtc,
			message.AttemptCount);

	public static InboxMessageDetails ToDetails(InboxMessage message) =>
		new(
			message.Id,
			message.TenantId,
			message.PublicReference,
			message.Type,
			message.PayloadJson,
			message.Source,
			message.RequestPath,
			message.HttpMethod,
			message.RemoteIpAddress,
			message.UserAgent,
			message.State,
			message.ReceivedAtUtc,
			message.ProcessingStartedAtUtc,
			message.ProcessedAtUtc,
			message.AttemptCount,
			message.NextAttemptAtUtc,
			message.LastError,
			message.IdempotencyKey);
}
