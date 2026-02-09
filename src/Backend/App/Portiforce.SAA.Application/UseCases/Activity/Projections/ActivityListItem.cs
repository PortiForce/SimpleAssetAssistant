using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Projections;

public sealed record ActivityListItem(
	ActivityId Id,
	DateTimeOffset OccurredAt,
	AssetActivityKind Kind,
	string Description,
	string PlatformName) : IListItemProjection;
