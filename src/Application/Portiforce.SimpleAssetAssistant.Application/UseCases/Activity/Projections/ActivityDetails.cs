using Portiforce.SimpleAssetAssistant.Application.Interfaces.Projections;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Projections;

public sealed record ActivityDetails(
	ActivityId Id,
	DateTimeOffset OccurredAt,
	AssetActivityKind Kind,
	string Description,
	string PlatformName) : IDetailsProjection;
