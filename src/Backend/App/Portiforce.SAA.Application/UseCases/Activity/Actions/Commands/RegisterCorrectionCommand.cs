using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Actions.Commands;

public sealed record RegisterCorrectionCommand(
	TenantId TenantId,
	PlatformAccountId PlatformAccountId,
	DateTimeOffset OccurredAt,
	AssetId AssetId,
	QuantityDelta DeltaAmount, // Not Quantity, as this might accept negative values as a correction element
	AssetActivityReason Reason,
	string? Notes
) : ICommand<CommandResult<ActivityId>>;
