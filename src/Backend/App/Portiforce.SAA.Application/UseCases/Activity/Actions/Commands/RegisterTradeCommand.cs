using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Actions.Commands;

/// <summary>
/// A flat command representing a Trade. 
/// The Handler will convert this into the complex "Legs" structure.
/// </summary>
public sealed record RegisterTradeCommand(
	AccountId AccountId,
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
	ExternalMetadata Metadata,
	CompletionType CompletionType
) : ICommand<CommandResult<ActivityId>>;
