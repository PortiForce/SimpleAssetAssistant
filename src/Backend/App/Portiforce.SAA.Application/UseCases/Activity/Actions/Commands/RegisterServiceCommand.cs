using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Actions.Commands;

public sealed record RegisterServiceCommand(
	TenantId TenantId,
	PlatformAccountId PlatformAccountId,
	DateTimeOffset OccurredAt,
	ServiceType ServiceType,
	AssetId AssetId,
	Quantity Amount,
	AssetActivityReason FeeReason,
	ExternalMetadata Metadata
) : ICommand<CommandResult<ActivityId>>;
