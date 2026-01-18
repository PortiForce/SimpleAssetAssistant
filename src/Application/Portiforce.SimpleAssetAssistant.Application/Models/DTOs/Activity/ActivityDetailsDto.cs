using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Activity;

public sealed record ActivityDetailsDto(
	ActivityId Id,
	DateTimeOffset OccurredAt,
	AssetActivityKind Kind,
	string Description,
	string PlatformName
);
