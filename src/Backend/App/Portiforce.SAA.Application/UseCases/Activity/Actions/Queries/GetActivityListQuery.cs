using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Activity.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Actions.Queries;

public sealed record GetActivityListQuery(
	AccountId AccountId,
	TenantId TenantId,
	PageRequest PageRequest,
	DateTimeOffset? FromDate = null,
	DateTimeOffset? ToDate = null,
	string? AssetCode = null
) : IQuery<PagedResult<ActivityListItem>>;
