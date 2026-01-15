using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;

/// <summary>
/// A flat command representing an Exchange. 
/// The Handler will convert this into the complex "Legs" structure.
/// </summary>
public sealed record RegisterExchangeCommand(
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	DateTimeOffset OccurredAt,

	// Trade Details
	string Pair, // e.g. "BTC/USD"
	MarketKind MarketKind,
	TradeExecutionType ExecutionType,

	// Inflow (What I got)
	string InAssetCode,
	decimal InAmount,

	// Outflow (What I gave)
	string OutAssetCode,
	decimal OutAmount,

	// Fee (Optional)
	string? FeeAssetCode,
	decimal? FeeAmount,

	// Metadata
	string Source,
	string? ExternalId
) : ICommand<BaseCreateCommandResponse<ActivityId>>;
