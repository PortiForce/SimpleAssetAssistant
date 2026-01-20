using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;

/// <summary>
/// A flat command representing an Exchange. 
/// The Handler will convert this into the complex "Legs" structure.
/// </summary>
public sealed record RegisterExchangeCommand(
	TenantId TenantId,
	PlatformAccountId PlatformAccountId,
	DateTimeOffset OccurredAt,
	MarketKind MarketKind,
	TradeExecutionType ExecutionType,
	AssetId InAssetId,
	Quantity InAmount,
	AssetId OutAssetId,
	Quantity OutAmount,
	AssetId? FeeAssetId,
	Quantity? FeeAmount,
	ExchangeType Type,
	ExternalMetadata Metadata
) : ICommand<BaseCreateCommandResult<ActivityId>>;
