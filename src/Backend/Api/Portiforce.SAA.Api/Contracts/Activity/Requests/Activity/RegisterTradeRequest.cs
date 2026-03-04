using Portiforce.SAA.Core.Activities.Enums;

namespace Portiforce.SAA.Api.Contracts.Activity.Requests.Activity;

public sealed record RegisterTradeRequest(
	Guid PlatformAccountId,
	DateTimeOffset OccurredAt,
	Guid InAssetId,
	decimal InAmount,
	Guid OutAssetId,
	decimal OutAmount,
	Guid? FeeAssetId,
	decimal? FeeAmount,
	MarketKind MarketKind,
	TradeExecutionType ExecutionType,
	CompletionType CompletionType,
	string Source,
	string? ExternalId
);
