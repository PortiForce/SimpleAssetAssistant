using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Activity.Requests.Activity;

public sealed record RegisterExchangeRequest(
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
	ExchangeType ExchangeType,
	CompletionType CompletionType,
	string Source,
	string? ExternalId
);
