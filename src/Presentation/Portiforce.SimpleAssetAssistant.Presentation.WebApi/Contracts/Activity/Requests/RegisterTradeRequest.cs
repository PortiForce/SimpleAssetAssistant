using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Activity.Requests;

public sealed record RegisterTradeRequest(
	Guid TenantId,
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
	string Source,
	string? ExternalId
);
