using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Actions.Commands;

public sealed record RegisterTransferCommand(
	TenantId TenantId,
	PlatformAccountId PlatformAccountId,
	DateTimeOffset OccurredAt,
	AssetId AssetId,
	Quantity Amount,
	MovementDirection Direction,
	AssetId? FeeAssetId,
	Quantity? FeeAmount,
	string? Reference,
	string? Counterparty,
	string Source
) : ICommand<CommandResult<ActivityId>>;
