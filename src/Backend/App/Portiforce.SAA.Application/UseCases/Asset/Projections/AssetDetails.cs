using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Asset.Projections;

public sealed record AssetDetails(
	AssetId Id,
	AssetCode Code,
	string Name,
	AssetKind Kind,
	byte NativeDecimals,
	AssetLifecycleState State) : IDetailsProjection;
