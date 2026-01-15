using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Client.Tenant;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Actions.Queries;

public sealed record GetTenantListRequest(PageRequest PageRequest)
	: IQuery<PagedResult<TenantListItemDto>>;

