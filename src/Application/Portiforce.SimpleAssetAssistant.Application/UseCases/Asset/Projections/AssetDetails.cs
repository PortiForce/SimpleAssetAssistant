using Portiforce.SimpleAssetAssistant.Application.Interfaces.Projections;
using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Projections;

public sealed record AssetDetails(
	AssetId Id,
	AssetCode Code,
	string Name,
	AssetKind Kind,
	byte NativeDecimals,
	AssetLifecycleState State) : IDetailsProjection;
