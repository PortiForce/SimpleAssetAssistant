using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Platform;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Actions.Queries;

// Returns system-wide supported platforms
public sealed record GetPlatformsRequest(
	PageRequest PageRequest
) : IQuery<PagedResult<PlatformListItemDto>>;
