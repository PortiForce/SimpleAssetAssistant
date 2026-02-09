using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;

namespace Portiforce.SAA.Application.UseCases.Client.Tenant.Actions.Queries;

public sealed record GetTenantListQuery(PageRequest PageRequest)
	: IQuery<PagedResult<TenantListItem>>;

