using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Actions.Queries;

// Returns system-wide supported platforms
public sealed record GetPlatformListQuery(
	TenantId TenantId,
	PageRequest PageRequest
) : IQuery<PagedResult<PlatformListItem>>;
