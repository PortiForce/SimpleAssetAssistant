using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Platform.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Platform.Actions.Queries;

// Returns system-wide supported platforms
public sealed record GetPlatformListQuery(
	TenantId TenantId,
	PageRequest PageRequest
) : IQuery<PagedResult<PlatformListItem>>;
