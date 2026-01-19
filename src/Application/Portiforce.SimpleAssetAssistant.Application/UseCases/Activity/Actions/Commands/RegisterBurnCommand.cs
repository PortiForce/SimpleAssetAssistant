using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;

public sealed record RegisterBurnCommand(
	TenantId TenantId,
	PlatformAccountId PlatformAccountId,
	DateTimeOffset OccurredAt,
	AssetId AssetId,
	Quantity Amount,
	AssetActivityReason BurnReason,
	string? Notes,
	ExternalMetadata Metadata
) : ICommand<BaseCreateCommandResult<ActivityId>>;
