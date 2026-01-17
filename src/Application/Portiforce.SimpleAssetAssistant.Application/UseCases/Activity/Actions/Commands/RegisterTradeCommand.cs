using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;

/// <summary>
/// A flat command representing a Trade. 
/// The Handler will convert this into the complex "Legs" structure.
/// </summary>
public sealed record RegisterTradeCommand(
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	DateTimeOffset OccurredAt,
	string Pair,
	string MarketKind,
	string ExecutionType,
	string InAssetCode,
	decimal InAmount,
	string OutAssetCode,
	decimal OutAmount,
	string? FeeAssetCode,
	decimal? FeeAmount,
	string Source,
	string? ExternalId
) : ICommand<BaseCreateCommandResponse<ActivityId>>;
