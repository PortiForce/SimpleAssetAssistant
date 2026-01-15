using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Activity;

public sealed record ActivityListItemDto(
	ActivityId Id,
	DateTimeOffset OccurredAt,
	string Type,              // "Trade", "Transfer"
	string Description,       // Generated summary: "Bought 1 BTC for 50k USD"
	string PlatformName
);
