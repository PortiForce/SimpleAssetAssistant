using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Messaging.Result;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Messaging.Actions.Commands;

public sealed record CreateInboxMessageCommand(
	TenantId TenantId,
	string Type,
	string PayloadJson,
	string Source,
	string RequestPath,
	string HttpMethod,
	string IdempotencyKey,
	DateTimeOffset ReceivedAtUtc,
	string? RemoteIpAddress = null,
	string? UserAgent = null) : ICommand<TypedResult<CreateInboxMessageResult>>;
