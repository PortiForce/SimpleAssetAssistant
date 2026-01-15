using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Asset;
public sealed record AssetListItemDto(
	AssetId Id,
	string Code,
	string Name,
	string Kind,
	int Decimals,
	string State
);
