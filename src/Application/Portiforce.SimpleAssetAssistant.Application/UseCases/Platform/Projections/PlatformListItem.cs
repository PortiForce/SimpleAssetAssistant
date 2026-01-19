using Portiforce.SimpleAssetAssistant.Application.Interfaces.Projections;
using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Projections;

public sealed record PlatformListItem(
	PlatformId Id,
	string Code,
	string Name,
	PlatformState State) : IListItemProjection;
