using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Asset;
public sealed record AssetListItemDto(
	AssetId Id,
	AssetCode Code,
	string Name,
	AssetKind Kind,
	byte NativeDecimals,
	AssetLifecycleState State
);
