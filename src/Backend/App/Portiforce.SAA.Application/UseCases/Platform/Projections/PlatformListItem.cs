using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Platform.Projections;

public sealed record PlatformListItem(
	PlatformId Id,
	string Code,
	string Name,
	PlatformState State) : IListItemProjection;
