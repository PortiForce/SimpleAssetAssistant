namespace Portiforce.SAA.Application.UseCases.Messaging.Result;

public sealed record CreateInboxMessageResult(
	Guid InboxMessageId,
	string PublicReference,
	DateTimeOffset ReceivedAtUtc);
