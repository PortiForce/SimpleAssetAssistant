using Portiforce.SimpleAssetAssistant.Core.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Asset;

public sealed record AssetDetailsDto(
	AssetId Id,
	string Code,
	string Name,
	string Kind,
	int Decimals,
	EntityLifecycleState State
);
