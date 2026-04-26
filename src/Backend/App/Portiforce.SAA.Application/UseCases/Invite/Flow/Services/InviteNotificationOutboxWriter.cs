using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Notification;
using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Outbox;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Application.Models.Invite;
using Portiforce.SAA.Application.Tech.Abstractions.Serialization;

namespace Portiforce.SAA.Application.UseCases.Invite.Flow.Services;

internal sealed class InviteNotificationOutboxWriter(
	IOutboxMessageWriteRepository outboxMessageWriteRepository,
	IClock clock,
	IJsonSerializer jsonSerializer)
	: IInviteNotificationOutboxWriter
{
	public async ValueTask AddInviteEmailAsync(
		SendInviteByChannelMessage message,
		CancellationToken ct)
	{
		string payloadJson = jsonSerializer.Serialize(message);

		string idempotencyKey =
			$"InviteEmail:{message.TenantId.Value}:{message.InviteId}";

		OutboxMessage outboxMessage = OutboxMessage.Create(
			message.TenantId,
			MessageTypes.SendInviteEmailV1,
			payloadJson,
			idempotencyKey,
			clock.UtcNow);

		await outboxMessageWriteRepository.AddAsync(outboxMessage, ct);
	}
}