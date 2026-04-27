using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.BackgroundTasks.Inbox;
using Portiforce.SAA.Application.Models.Common.Messaging;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Messaging.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Messaging.Result;
using Portiforce.SAA.Core.Extensions;

namespace Portiforce.SAA.Application.UseCases.Messaging.Handlers.Commands;

internal sealed class CreateInboxMessageCommandHandler(
	IInboxMessageWriteRepository inboxMessageWriteRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateInboxMessageCommand, TypedResult<CreateInboxMessageResult>>
{
	public async ValueTask<TypedResult<CreateInboxMessageResult>> Handle(
		CreateInboxMessageCommand request,
		CancellationToken ct)
	{
		if (request.TenantId.IsEmpty)
		{
			return TypedResult<CreateInboxMessageResult>.Fail(ResultError.Validation("TenantId is not defined."));
		}

		if (request.ReceivedAtUtc == default)
		{
			return TypedResult<CreateInboxMessageResult>.Fail(ResultError.Validation("ReceivedAtUtc is not defined."));
		}

		try
		{
			Guid inboxMessageId = GuidExtensions.New();
			string publicReference = CreatePublicReference(inboxMessageId, request.ReceivedAtUtc);

			InboxMessage inboxMessage = InboxMessage.Create(
				request.TenantId,
				publicReference,
				request.Type,
				request.PayloadJson,
				request.Source,
				request.RequestPath,
				request.HttpMethod,
				request.IdempotencyKey,
				request.ReceivedAtUtc,
				request.RemoteIpAddress,
				request.UserAgent,
				inboxMessageId);

			await inboxMessageWriteRepository.AddAsync(inboxMessage, ct);
			_ = await unitOfWork.SaveChangesAsync(ct);

			return TypedResult<CreateInboxMessageResult>.Ok(
				new CreateInboxMessageResult(
					inboxMessage.Id,
					inboxMessage.PublicReference,
					inboxMessage.ReceivedAtUtc));
		}
		catch (ArgumentException ex)
		{
			return TypedResult<CreateInboxMessageResult>.Fail(ResultError.Validation(ex.Message));
		}
		catch (UniqueConstraintViolationException)
		{
			return TypedResult<CreateInboxMessageResult>.Fail(
				ResultError.Conflict("Inbox message could not be created because the idempotency key already exists."));
		}
	}

	private static string CreatePublicReference(Guid inboxMessageId, DateTimeOffset receivedAtUtc)
	{
		string datePart = receivedAtUtc.UtcDateTime.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
		string idPart = inboxMessageId.ToString("N")[..12].ToUpperInvariant();

		return $"MSG-{datePart}-{idPart}";
	}
}
