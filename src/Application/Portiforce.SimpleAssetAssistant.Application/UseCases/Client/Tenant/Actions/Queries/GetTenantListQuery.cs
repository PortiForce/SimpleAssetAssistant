using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Actions.Queries;

public sealed record GetTenantListQuery(PageRequest PageRequest)
	: IQuery<PagedResult<TenantListItem>>;

