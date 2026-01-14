using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Models.Futures;

public sealed record FuturesDescriptor
{
	public required string InstrumentKey { get; init; }
	public FuturesContractKind ContractKind { get; init; } = FuturesContractKind.Perpetual;

	public string? BaseAssetCode { get; init; }
	public string? QuoteAssetCode { get; init; }

	public FuturesPositionEffect? PositionEffect { get; init; }
}
