using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Activity;

public record RegisterTradeDto
{
	public TenantId TenantId { get; init; }
	public AccountId AccountId { get; init; }
	public PlatformId PlatformId { get; init; }
	public DateTimeOffset OccurredAt { get; init; }

	// Trade Details
	public string Pair { get; init; } // e.g. "BTC/USD"
	public MarketKind MarketKind { get; init; }
	public TradeExecutionType ExecutionType { get; init; }

	// Inflow (What I got)
	public string InAssetCode { get; init; }
	public decimal InAmount { get; init; }

	// Outflow (What I gave)
	public string OutAssetCode { get; init; }
	public decimal OutAmount { get; init; }

	// Fee (Optional)
	public string? FeeAssetCode { get; init; }
	public decimal? FeeAmount { get; init; }

	// Metadata
	public string Source { get; init; }
	public string? ExternalId { get; init; }
}
