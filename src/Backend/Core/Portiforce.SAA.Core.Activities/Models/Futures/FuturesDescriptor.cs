using Portiforce.SAA.Core.Activities.Enums;

namespace Portiforce.SAA.Core.Activities.Models.Futures;

public sealed record FuturesDescriptor
{
	private FuturesDescriptor() { }

	private FuturesDescriptor(
		string instrumentKey,
		FuturesContractKind contractKind,
		string baseAssetCode,
		string quoteAssetCode,
		FuturesPositionEffect positionsEffect)
	{
		InstrumentKey = instrumentKey;
		ContractKind = contractKind;
		BaseAssetCode = baseAssetCode;
		QuoteAssetCode = quoteAssetCode;
		PositionEffect = positionsEffect;
	}

	public string InstrumentKey { get; init; }
	public FuturesContractKind ContractKind { get; init; } = FuturesContractKind.Perpetual;

	public string? BaseAssetCode { get; init; }
	public string? QuoteAssetCode { get; init; }

	public FuturesPositionEffect? PositionEffect { get; init; }

	public static FuturesDescriptor Create(
		string instrumentKey,
		FuturesContractKind contractKind,
		string baseAssetCode,
		string quoteAssetCode,
		FuturesPositionEffect positionsEffect)
	{
		return new(
			instrumentKey,
			contractKind,
			baseAssetCode,
			quoteAssetCode,
			positionsEffect);
	}
}
